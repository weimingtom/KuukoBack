/*
 ** $Id: ldump.c,v 2.8.1.1 2007/12/27 13:02:25 roberto Exp $
 ** save precompiled Lua chunks
 ** See Copyright Notice in lua.h
 */
using System;

namespace kurumi
{
	//using lua_Number = System.Double;
	//using TValue = Lua.TValue;

	public class LuaDump
	{
	    public class DumpState
	    {
	        public LuaState.lua_State L;
	        public Lua.lua_Writer writer;
	        public object data;
	        public int strip;
	        public int status;
	    }		
		
		public static void DumpMem(object b, DumpState D, ClassType t)
		{
            byte[] bytes = t.ObjToBytes(b, t, 0);
			char[] ch = new char[bytes.Length];
			for (int i = 0; i < bytes.Length; i++)
			{
				ch[i] = (char)bytes[i];
			}
			CLib.CharPtr str = CLib.CharPtr.toCharPtr(ch);
			DumpBlock(str, /*(uint)*/str.chars.Length, D);
		}

		public static void DumpMem_int(int[] b, int n, DumpState D, ClassType t)
		{
            ClassType.Assert(b.Length == n);
			for (int i = 0; i < n; i++)
			{
				DumpMem(b[i], D, t);
			}
		}

        public static void DumpMem_long(long[] b, int n, DumpState D, ClassType t)
        {
            ClassType.Assert(b.Length == n);
            for (int i = 0; i < n; i++)
            {
                DumpMem(b[i], D, t);
            }
        }

		public static void DumpVar(object x, DumpState D, ClassType t)
		{
			DumpMem(x, D, t);
		}

		private static void DumpBlock(CLib.CharPtr b, int/*uint*/ size, DumpState D)
		{
			if (D.status == 0)
			{
				LuaLimits.lua_unlock(D.L);
				D.status = D.writer.exec(D. L, b, size, D.data);
				LuaLimits.lua_lock(D.L);
			}
		}

		private static void DumpChar(int y, DumpState D)
		{
			char x = (char)y;
			DumpVar(x, D, new ClassType(ClassType.TYPE_CHAR));
		}

		private static void DumpInt(int x, DumpState D)
		{
			DumpVar(x, D, new ClassType(ClassType.TYPE_INT));
		}

		private static void DumpNumber(Double/*lua_Number*/ x, DumpState D)
		{
			DumpVar(x, D, new ClassType(ClassType.TYPE_DOUBLE));
		}

		static void DumpVector_int(int[] b, int n, DumpState D, ClassType t)
		{
			DumpInt(n, D);
			DumpMem_int(b, n, D, t);
		}

        static void DumpVector_long(long[] b, int n, DumpState D, ClassType t)
        {
            DumpInt(n, D);
            DumpMem_long(b, n, D, t);
        }

		private static void DumpString(LuaObject.TString s, DumpState D)
		{
			if (s == null || CLib.CharPtr.isEqual(LuaObject.getstr(s), null))
			{
				int/*uint*/ size = 0;
				DumpVar(size, D, new ClassType(ClassType.TYPE_INT));
			}
			else
			{
				int/*uint*/ size = s.getTsv().len + 1;		/* include trailing '\0' */
				DumpVar(size, D, new ClassType(ClassType.TYPE_INT));
				DumpBlock(LuaObject.getstr(s), size, D);
			}
		}

		private static void DumpCode(LuaObject.Proto f, DumpState D)
		{
			DumpVector_long(f.code, f.sizecode, D, new ClassType(ClassType.TYPE_LONG));
		}

		private static void DumpConstants(LuaObject.Proto f, DumpState D)
		{
			int i, n = f.sizek;
			DumpInt(n, D);
			for (i = 0; i < n; i++)
			{
				/*const*/ LuaObject.TValue o = f.k[i];
				DumpChar(LuaObject.ttype(o), D);
				switch (LuaObject.ttype(o))
				{
					case Lua.LUA_TNIL:
						{
							break;
						}
					case Lua.LUA_TBOOLEAN:
						{
							DumpChar(LuaObject.bvalue(o), D);
							break;
						}
					case Lua.LUA_TNUMBER:
						{
							DumpNumber(LuaObject.nvalue(o), D);
							break;
						}
					case Lua.LUA_TSTRING:
						{
							DumpString(LuaObject.rawtsvalue(o), D);
							break;
						}
					default:
						{
							LuaLimits.lua_assert(0);			/* cannot happen */
							break;
						}
				}
			}
			n = f.sizep;
			DumpInt(n, D);
			for (i = 0; i < n; i++) 
			{
				DumpFunction(f.p[i],f.source,D);
			}
		}

		private static void DumpDebug(LuaObject.Proto f, DumpState D)
		{
			int i,n;
			n = (D.strip != 0) ? 0 : f.sizelineinfo;
			DumpVector_int(f. lineinfo, n, D, new ClassType(ClassType.TYPE_INT));
			n = (D.strip != 0) ? 0 : f.sizelocvars;
			DumpInt(n, D);
			for (i = 0; i < n; i++)
			{
				DumpString(f.locvars[i].varname, D);
				DumpInt(f.locvars[i].startpc, D);
				DumpInt(f.locvars[i].endpc, D);
			}
			n = (D.strip != 0) ? 0 : f.sizeupvalues;
			DumpInt(n, D);
			for (i = 0; i < n; i++) 
			{
				DumpString(f.upvalues[i], D);
			}
		}

		private static void DumpFunction(LuaObject.Proto f, LuaObject.TString p, DumpState D)
		{
			DumpString(((f.source == p) || (D.strip != 0)) ? null : f.source, D);
			DumpInt(f.linedefined, D);
			DumpInt(f.lastlinedefined, D);
			DumpChar(f.nups, D);
			DumpChar(f.numparams, D);
			DumpChar(f.is_vararg, D);
			DumpChar(f.maxstacksize, D);
			DumpCode(f, D);
			DumpConstants(f, D);
			DumpDebug(f, D);
		}

		private static void DumpHeader(DumpState D)
		{
			CLib.CharPtr h = CLib.CharPtr.toCharPtr(new char[LuaUndump.LUAC_HEADERSIZE]);
			LuaUndump.luaU_header(h);
			DumpBlock(h, LuaUndump.LUAC_HEADERSIZE, D);
		}

		/*
		 ** dump Lua function as precompiled chunk
		 */
		public static int luaU_dump (LuaState.lua_State L, LuaObject.Proto f, Lua.lua_Writer w, object data, int strip)
		{
			DumpState D = new DumpState();
			D.L = L;
			D.writer = w;
			D.data = data;
			D.strip = strip;
			D.status = 0;
			DumpHeader(D);
			DumpFunction(f, null, D);
			return D.status;
		}
	}
}
