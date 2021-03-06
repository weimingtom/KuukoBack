/*
 ** $Id: ltm.c,v 2.8.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Tag methods
 ** See Copyright Notice in lua.h
 */
using System;
 
namespace kurumi
{
	//using TValue = Lua.TValue;

	public class LuaTM
	{
		/*
		 * WARNING: if you change the order of this enumeration,
		 * grep "ORDER TM"
		 */
		public enum TMS
		{
			TM_INDEX,
			TM_NEWINDEX,
			TM_GC,
			TM_MODE,
			TM_EQ,  /* last tag method with `fast' access */
			TM_ADD,
			TM_SUB,
			TM_MUL,
			TM_DIV,
			TM_MOD,
			TM_POW,
			TM_UNM,
			TM_LEN,
			TM_LT,
			TM_LE,
			TM_CONCAT,
			TM_CALL,
			TM_N		/* number of elements in the enum */
		}
		
       	public static int convertTMStoInt(LuaTM.TMS tms)
        {
            switch (tms)
            {
                case LuaTM.TMS.TM_INDEX:
                    return 0;
                case LuaTM.TMS.TM_NEWINDEX:
                    return 1;
                case LuaTM.TMS.TM_GC:
                    return 2;
                case LuaTM.TMS.TM_MODE:
                    return 3;
                case LuaTM.TMS.TM_EQ:
                    return 4;
                case LuaTM.TMS.TM_ADD:
                    return 5;
                case LuaTM.TMS.TM_SUB:
                    return 6;
                case LuaTM.TMS.TM_MUL:
                    return 7;
                case LuaTM.TMS.TM_DIV:
                    return 8;
                case LuaTM.TMS.TM_MOD:
                    return 9;
                case LuaTM.TMS.TM_POW:
                    return 10;
                case LuaTM.TMS.TM_UNM:
                    return 11;
                case LuaTM.TMS.TM_LEN:
                    return 12;
                case LuaTM.TMS.TM_LT:
                    return 13;
                case LuaTM.TMS.TM_LE:
                    return 14;
                case LuaTM.TMS.TM_CONCAT:
                    return 15;
                case LuaTM.TMS.TM_CALL:
                    return 16;
                case LuaTM.TMS.TM_N:
                    return 17;
            }
            throw new Exception("convertTMStoInt error");
        }
		
		public static LuaObject.TValue gfasttm(LuaState.global_State g, LuaObject.Table et, TMS e)
		{
			return (et == null) ? null :
				((et.flags & (1 << (int)e)) != 0) ? null :
				luaT_gettm(et, e, g.tmname[(int)e]);
		}

		public static LuaObject.TValue fasttm(LuaState.lua_State l, LuaObject.Table et, TMS e) 
		{ 
			return gfasttm(LuaState.G(l), et, e); 
		}

		public readonly static CLib.CharPtr[] luaT_typenames = {
			CLib.CharPtr.toCharPtr("nil"), 
			CLib.CharPtr.toCharPtr("boolean"), 
			CLib.CharPtr.toCharPtr("userdata"), 
			CLib.CharPtr.toCharPtr("number"),
			CLib.CharPtr.toCharPtr("string"), 
			CLib.CharPtr.toCharPtr("table"), 
			CLib.CharPtr.toCharPtr("function"), 
			CLib.CharPtr.toCharPtr("userdata"), 
			CLib.CharPtr.toCharPtr("thread"),
			CLib.CharPtr.toCharPtr("proto"), 
			CLib.CharPtr.toCharPtr("upval")
		};

		private readonly static CLib.CharPtr[] luaT_eventname = {  /* ORDER TM */
			CLib.CharPtr.toCharPtr("__index"), 
			CLib.CharPtr.toCharPtr("__newindex"),
			CLib.CharPtr.toCharPtr("__gc"), 
			CLib.CharPtr.toCharPtr("__mode"), 
			CLib.CharPtr.toCharPtr("__eq"),
			CLib.CharPtr.toCharPtr("__add"), 
			CLib.CharPtr.toCharPtr("__sub"), 
			CLib.CharPtr.toCharPtr("__mul"), 
			CLib.CharPtr.toCharPtr("__div"), 
			CLib.CharPtr.toCharPtr("__mod"),
			CLib.CharPtr.toCharPtr("__pow"), 
			CLib.CharPtr.toCharPtr("__unm"), 
			CLib.CharPtr.toCharPtr("__len"), 
			CLib.CharPtr.toCharPtr("__lt"), 
			CLib.CharPtr.toCharPtr("__le"),
			CLib.CharPtr.toCharPtr("__concat"), 
			CLib.CharPtr.toCharPtr("__call")
		};

		public static void luaT_init(LuaState.lua_State L) 
		{
			int i;
			for (i = 0; i < (int)TMS.TM_N; i++) 
			{
				LuaState.G(L).tmname[i] = LuaString.luaS_new(L, luaT_eventname[i]);
				LuaString.luaS_fix(LuaState.G(L).tmname[i]);  /* never collect these names */
			}
		}

		/*
		 ** function to be used with macro "fasttm": optimized for absence of
		 ** tag methods
		 */
		public static LuaObject.TValue luaT_gettm(LuaObject.Table events, TMS event_, LuaObject.TString ename) 
		{
			//const
			LuaObject.TValue tm = LuaTable.luaH_getstr(events, ename);
			LuaLimits.lua_assert(convertTMStoInt(event_) <= convertTMStoInt(TMS.TM_EQ));
			if (LuaObject.ttisnil(tm))
			{  
				/* no tag method? */
				events.flags |= (byte)(1 << (int)event_);  /* cache this fact */
				return null;
			}
			else 
			{
				return tm;
			}
		}

		public static LuaObject.TValue luaT_gettmbyobj(LuaState.lua_State L, LuaObject.TValue o, TMS event_) 
		{
			LuaObject.Table mt;
			switch (LuaObject.ttype(o))
			{
				case Lua.LUA_TTABLE:
					{
						mt = LuaObject.hvalue(o).metatable;
						break;
					}
				case Lua.LUA_TUSERDATA:
					{
						mt = LuaObject.uvalue(o).metatable;
						break;
					}
				default:
					{
						mt = LuaState.G(L).mt[LuaObject.ttype(o)];
						break;
					}
			}
			return ((mt != null) ? LuaTable.luaH_getstr(mt, LuaState.G(L).tmname[(int)event_]) : LuaObject.luaO_nilobject);
		}
	}
}
