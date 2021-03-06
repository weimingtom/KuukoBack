/*
 ** $Id: lstring.c,v 2.8.1.1 2007/12/27 13:02:25 roberto Exp $
 ** String table (keeps all strings handled by Lua)
 ** See Copyright Notice in lua.h
 */
using System;

namespace kurumi
{
	//using lu_byte = System.Byte;

	public class LuaString
	{
		public static int sizestring(LuaObject.TString s) 
		{ 
			return ((int)s.len + 1) * CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_CHAR)); //char 
		}

		public static int sizeudata(LuaObject.Udata u) 
		{ 
			return (int)u.len; 
		}

		public static LuaObject.TString luaS_new(LuaState.lua_State L, CLib.CharPtr s) 
		{ 
			return luaS_newlstr(L, s, /*(uint)*/CLib.strlen(s)); 
		}
		
		public static LuaObject.TString luaS_newliteral(LuaState.lua_State L, CLib.CharPtr s) 
		{ 
			return luaS_newlstr(L, s, /*(uint)*/CLib.strlen(s)); 
		}

		public static void luaS_fix(LuaObject.TString s)
		{
			Byte/*lu_byte*/ marked = s.getTsv().marked;	// can't pass properties in as ref
			Byte[] marked_ref = new Byte[1];
			marked_ref[0] = marked;
			LuaGC.l_setbit(/*ref*/ marked_ref, LuaGC.FIXEDBIT);
			marked = marked_ref[0];
			s.getTsv().marked = marked;
		}

		public static void luaS_resize(LuaState.lua_State L, int newsize) 
		{
			LuaState.GCObject[] newhash;
			LuaState.stringtable tb;
			int i;
			if (LuaState.G(L).gcstate == LuaGC.GCSsweepstring)
			{
				return;  /* cannot resize during GC traverse */
			}
			
			// todo: fix this up
			// I'm treating newhash as a regular C# array, but I need to allocate a dummy array
			// so that the garbage collector behaves identical to the C version.
			//newhash = luaM_newvector<GCObjectRef>(L, newsize);
			newhash = new LuaState.GCObject[newsize];
            LuaMem.AddTotalBytes(L, newsize * CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_GCOBJECTREF))); //typeof(GCObjectRef)

			tb = LuaState.G(L).strt;
			for (i = 0; i < newsize; i++) 
			{
				newhash[i] = null;
			}

			/* rehash */
			for (i = 0; i < tb.size; i++) 
			{
				LuaState.GCObject p = tb.hash[i];
				while (p != null) 
				{  
					/* for each node in the list */
                    LuaState.GCObject next = p.getGch().next;  /* save next */
					long/*int*//*uint*/ h = LuaState.gco2ts(p).hash;
					int h1 = (int)CLib.lmod(h, newsize);  /* new position */
					LuaLimits.lua_assert((int)(h % newsize) == CLib.lmod(h, newsize));
                    p.getGch().next = newhash[h1];  /* chain it */
					newhash[h1] = p;
					p = next;
				}
			}
			//luaM_freearray(L, tb.hash);
			if (tb.hash != null)
			{
                LuaMem.SubtractTotalBytes(L, tb.hash.Length * CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_GCOBJECTREF))); //typeof(GCObjectRef)
			}
			tb.size = newsize;
			tb.hash = newhash;
		}

		public static LuaObject.TString newlstr(LuaState.lua_State L, CLib.CharPtr str, int/*uint*/ l, long/*int*//*uint*/ h) 
		{
			LuaObject.TString ts;
			LuaState.stringtable tb;
            if (l + 1 > LuaLimits.MAX_SIZET / CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_CHAR))) //typeof(char)
			{
				LuaMem.luaM_toobig(L);
			}
			ts = new LuaObject.TString(CLib.CharPtr.toCharPtr(new char[l + 1]));
            LuaMem.AddTotalBytes(L, (int)(l + 1) * CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_CHAR)) + CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_TSTRING))); //typeof(TString)//typeof(char)
			ts.getTsv().len = l;
            ts.getTsv().hash = h;
            ts.getTsv().marked = LuaGC.luaC_white(LuaState.G(L));
            ts.getTsv().tt = Lua.LUA_TSTRING;
            ts.getTsv().reserved = 0;
			//memcpy(ts+1, str, l*GetUnmanagedSize(typeof(char)));
			CLib.memcpy_char(ts.str.chars, str.chars, str.index, (int)l);
			ts.str.set(l, '\0');  /* ending 0 */
			tb = LuaState.G(L).strt;
			h = (int/*uint*/)CLib.lmod(h, tb.size);
            ts.getTsv().next = tb.hash[(int)h];  /* chain new entry */
			tb.hash[(int)h] = LuaState.obj2gco(ts);
			tb.nuse++;
			if ((tb.nuse > (int)tb.size) && (tb.size <= LuaLimits.MAX_INT / 2))
			{
				luaS_resize(L, tb.size*2);  /* too crowded */
			}
			return ts;
		}

		public static LuaObject.TString luaS_newlstr(LuaState.lua_State L, CLib.CharPtr str, int/*uint*/ l) 
		{
			LuaState.GCObject o;
			/*FIXME:*/
			long/*int*//*uint*/ h = /*(uint)*/l & 0xffffffff;  /* seed */
			int/*uint*/ step = (l >> 5) + 1;  /* if string is too long, don't hash all its chars */
			int/*uint*/ l1;
			for (l1 = l; l1 >= step; l1 -= step) 
			{
				/*FIXME:*/
				/* compute hash */
				h = (0xffffffff) & (h ^ ((h << 5)+(h >> 2) + (byte)str.get(l1 - 1)));
			}
			for (o = LuaState.G(L).strt.hash[(int)CLib.lmod(h, LuaState.G(L).strt.size)];
			     o != null;
                 o = o.getGch().next) 
			{
				LuaObject.TString ts = LuaState.rawgco2ts(o);
                if (ts.getTsv().len == l && (CLib.memcmp(str, LuaObject.getstr(ts), l) == 0))
				{
					/* string may be dead */
					if (LuaGC.isdead(LuaState.G(L), o)) 
					{
						LuaGC.changewhite(o);
					}
					return ts;
				}
			}
			//return newlstr(L, str, l, h);  /* not found */
			LuaObject.TString res = newlstr(L, str, l, h);
			return res;
		}

		public static LuaObject.Udata luaS_newudata(LuaState.lua_State L, int/*uint*/ s, LuaObject.Table e)
		{
			LuaObject.Udata u = new LuaObject.Udata();
			u.uv.marked = LuaGC.luaC_white(LuaState.G(L));  /* is not finalized */
			u.uv.tt = Lua.LUA_TUSERDATA;
			u.uv.len = s;
			u.uv.metatable = null;
			u.uv.env = e;
			u.user_data = new byte[s];
			/* chain it on udata list (after main thread) */
			u.uv.next = LuaState.G(L).mainthread.next;
			LuaState.G(L).mainthread.next = LuaState.obj2gco(u);
			return u;
		}

        public static LuaObject.Udata luaS_newudata(LuaState.lua_State L, ClassType t, LuaObject.Table e)
		{
			LuaObject.Udata u = new LuaObject.Udata();
			u.uv.marked = LuaGC.luaC_white(LuaState.G(L));  /* is not finalized */
			u.uv.tt = Lua.LUA_TUSERDATA;
			u.uv.len = 0;
			u.uv.metatable = null;
			u.uv.env = e;
			u.user_data = LuaMem.luaM_realloc_(L, t);
            LuaMem.AddTotalBytes(L, CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_UDATA)));//typeof(Udata)
			/* chain it on udata list (after main thread) */
			u.uv.next = LuaState.G(L).mainthread.next;
			LuaState.G(L).mainthread.next = LuaState.obj2gco(u);
			return u;
		}
	}
}
