/*
 ** $Id: lfunc.c,v 2.12.1.2 2007/12/28 14:58:43 roberto Exp $
 ** Auxiliary functions to manipulate prototypes and closures
 ** See Copyright Notice in lua.h
 */
namespace kurumi
{
	//using TValue = Lua.TValue;
	//using StkId = TValue;
	//using Instruction = System.UInt32;

	public class LuaFunc
	{
		public static int sizeCclosure(int n) 
		{
            return CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_CCLOSURE)) + CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_TVALUE)) * (n - 1); //typeof(CClosure)//typeof(TValue)
		}

		public static int sizeLclosure(int n) 
		{
            return CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_LCLOSURE)) + CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_TVALUE)) * (n - 1); //typeof(LClosure)//typeof(TValue)
		}

		public static LuaObject.Closure luaF_newCclosure(LuaState.lua_State L, int nelems, LuaObject.Table e) 
		{
			//Closure c = (Closure)luaM_malloc(L, sizeCclosure(nelems));
			LuaObject.Closure c = LuaMem.luaM_new_Closure(L, new ClassType(ClassType.TYPE_CLOSURE));
			LuaMem.AddTotalBytes(L, sizeCclosure(nelems));
			LuaGC.luaC_link(L, LuaState.obj2gco(c), (byte)Lua.LUA_TFUNCTION);
			c.c.setIsC((byte)1);
			c.c.setEnv(e);
			c.c.setNupvalues(LuaLimits.cast_byte(nelems));
			c.c.upvalue = new LuaObject.TValue[nelems];
			for (int i = 0; i < nelems; i++)
			{
				c.c.upvalue[i] = new LuaObject.TValue();
			}
			return c;
		}

		public static LuaObject.Closure luaF_newLclosure(LuaState.lua_State L, int nelems, LuaObject.Table e) 
		{
			//Closure c = (Closure)luaM_malloc(L, sizeLclosure(nelems));
			LuaObject.Closure c = LuaMem.luaM_new_Closure(L, new ClassType(ClassType.TYPE_CLOSURE));
			LuaMem.AddTotalBytes(L, sizeLclosure(nelems));
			LuaGC.luaC_link(L, LuaState.obj2gco(c), (byte)Lua.LUA_TFUNCTION);
			c.l.setIsC((byte)0);
			c.l.setEnv(e);
			c.l.setNupvalues(LuaLimits.cast_byte(nelems));
			c.l.upvals = new LuaObject.UpVal[nelems];
			for (int i = 0; i < nelems; i++)
			{
				c.l.upvals[i] = new LuaObject.UpVal();
			}
			while (nelems-- > 0) 
			{
				c.l.upvals[nelems] = null;
			}
			return c;
		}

		public static LuaObject.UpVal luaF_newupval(LuaState.lua_State L) 
		{
			LuaObject.UpVal uv = LuaMem.luaM_new_UpVal(L, new ClassType(ClassType.TYPE_UPVAL));
			LuaGC.luaC_link(L, LuaState.obj2gco(uv), (byte)LuaObject.LUA_TUPVAL);
			uv.v = uv.u.value;
			LuaObject.setnilvalue(uv.v);
			return uv;
		}

		public static LuaObject.UpVal luaF_findupval(LuaState.lua_State L, LuaObject.TValue/*StkId*/ level)
		{
			LuaState.global_State g = LuaState.G(L);
			LuaState.GCObjectRef pp = new LuaState.OpenValRef(L);
			LuaObject.UpVal p;
			LuaObject.UpVal uv;
			while (pp.get() != null && LuaObject.TValue.greaterEqual((p = LuaState.ngcotouv(pp.get())).v, level))
			{
				LuaLimits.lua_assert(p.v != p.u.value);
				if (p.v == level) 
				{  
					/* found a corresponding upvalue? */
					if (LuaGC.isdead(g, LuaState.obj2gco(p)))  /* is it dead? */
					{
						LuaGC.changewhite(LuaState.obj2gco(p));  /* ressurect it */
					}
					return p;
				}
				pp = new LuaState.NextRef(p);
			}
			uv = LuaMem.luaM_new_UpVal(L, new ClassType(ClassType.TYPE_UPVAL));  /* not found: create a new one */
			uv.tt = LuaObject.LUA_TUPVAL;
			uv.marked = LuaGC.luaC_white(g);
			uv.v = level;  /* current value lives in the stack */
			uv.next = pp.get();  /* chain it in the proper position */
			pp.set(LuaState.obj2gco(uv));
			uv.u.l.prev = g.uvhead;  /* double link it in `uvhead' list */
			uv.u.l.next = g.uvhead.u.l.next;
			uv.u.l.next.u.l.prev = uv;
			g.uvhead.u.l.next = uv;
			LuaLimits.lua_assert(uv.u.l.next.u.l.prev == uv && uv.u.l.prev.u.l.next == uv);
			return uv;
		}

		private static void unlinkupval(LuaObject.UpVal uv) 
		{
			LuaLimits.lua_assert(uv.u.l.next.u.l.prev == uv && uv.u.l.prev.u.l.next == uv);
			uv.u.l.next.u.l.prev = uv.u.l.prev;  /* remove from `uvhead' list */
			uv.u.l.prev.u.l.next = uv.u.l.next;
		}

		public static void luaF_freeupval(LuaState.lua_State L, LuaObject.UpVal uv) 
		{
			if (uv.v != uv.u.value)  /* is it open? */
			{
				unlinkupval(uv);  /* remove from open list */
			}
			LuaMem.luaM_free_UpVal(L, uv, new ClassType(ClassType.TYPE_UPVAL));  /* free upvalue */
		}

		public static void luaF_close(LuaState.lua_State L, LuaObject.TValue/*StkId*/ level)
		{
			LuaObject.UpVal uv;
			LuaState.global_State g = LuaState.G(L);
			while (L.openupval != null && LuaObject.TValue.greaterEqual((uv = LuaState.ngcotouv(L.openupval)).v, level))
			{
				LuaState.GCObject o = LuaState.obj2gco(uv);
				LuaLimits.lua_assert(!LuaGC.isblack(o) && uv.v != uv.u.value);
				L.openupval = uv.next;  /* remove from `open' list */
				if (LuaGC.isdead(g, o))
				{
					luaF_freeupval(L, uv);  /* free upvalue */
				}
				else 
				{
					unlinkupval(uv);
					LuaObject.setobj(L, uv.u.value, uv.v);
					uv.v = uv.u.value;  /* now current value lives here */
					LuaGC.luaC_linkupval(L, uv);  /* link upvalue into `gcroot' list */
				}
			}
		}

		public static LuaObject.Proto luaF_newproto(LuaState.lua_State L) 
		{
			LuaObject.Proto f = LuaMem.luaM_new_Proto(L, new ClassType(ClassType.TYPE_PROTO));
			LuaGC.luaC_link(L, LuaState.obj2gco(f), (byte)LuaObject.LUA_TPROTO);
			f.k = null;
			f.sizek = 0;
			f.p = null;
			f.sizep = 0;
			f.code = null;
			f.sizecode = 0;
			f.sizelineinfo = 0;
			f.sizeupvalues = 0;
			f.nups = 0;
			f.upvalues = null;
			f.numparams = 0;
			f.is_vararg = 0;
			f.maxstacksize = 0;
			f.lineinfo = null;
			f.sizelocvars = 0;
			f.locvars = null;
			f.linedefined = 0;
			f.lastlinedefined = 0;
			f.source = null;
			return f;
		}

		public static void luaF_freeproto(LuaState.lua_State L, LuaObject.Proto f) 
		{
            /*UInt32*/
            /*Instruction*/
			LuaMem.luaM_freearray_long(L, f.code, new ClassType(ClassType.TYPE_LONG));
			LuaMem.luaM_freearray_Proto(L, f.p, new ClassType(ClassType.TYPE_PROTO));
			LuaMem.luaM_freearray_TValue(L, f.k, new ClassType(ClassType.TYPE_TVALUE));
            /*Int32*/
            LuaMem.luaM_freearray_int(L, f.lineinfo, new ClassType(ClassType.TYPE_INT32));
			LuaMem.luaM_freearray_LocVar(L, f.locvars, new ClassType(ClassType.TYPE_LOCVAR));
			LuaMem.luaM_freearray_TString(L, f.upvalues, new ClassType(ClassType.TYPE_TSTRING));
			LuaMem.luaM_free_Proto(L, f, new ClassType(ClassType.TYPE_PROTO));
		}

		// we have a gc, so nothing to do
		public static void luaF_freeclosure(LuaState.lua_State L, LuaObject.Closure c) 
		{
			int size = (c.c.getIsC() != 0) ? sizeCclosure(c.c.getNupvalues()) :
				sizeLclosure(c.l.getNupvalues());
			//luaM_freemem(L, c, size);
			LuaMem.SubtractTotalBytes(L, size);
		}

		/*
		 ** Look for n-th local variable at line `line' in function `func'.
		 ** Returns null if not found.
		 */
		public static CLib.CharPtr luaF_getlocalname(LuaObject.Proto f, int local_number, int pc) 
		{
			int i;
			for (i = 0; i<f.sizelocvars && f.locvars[i].startpc <= pc; i++) 
			{
				if (pc < f.locvars[i].endpc) 
				{  
					/* is variable active? */
					local_number--;
					if (local_number == 0)
					{
						return LuaObject.getstr(f.locvars[i].varname);
					}
				}
			}
			return null;  /* not found */
		}
	}
}
