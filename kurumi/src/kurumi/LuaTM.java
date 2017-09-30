package kurumi;

//
// ** $Id: ltm.c,v 2.8.1.1 2007/12/27 13:02:25 roberto Exp $
// ** Tag methods
// ** See Copyright Notice in lua.h
// 
//using TValue = Lua.TValue;

public class LuaTM {
	/*
	 * WARNING: if you change the order of this enumeration,
	 * grep "ORDER TM"
	 */
	public static enum TMS
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
		TM_N;		/* number of elements in the enum */

		public int getValue() {
			return this.ordinal();
		}

		public static TMS forValue(int value) {
			return values()[value];
		}
	}	
	
	public static TValue gfasttm(LuaState.global_State g, LuaObject.Table et, TMS e) {
		return (et == null) ? null : ((et.flags & (1 << e.getValue())) != 0) ? null : luaT_gettm(et, e, g.tmname[e.getValue()]);
	}

	public static TValue fasttm(LuaState.lua_State l, LuaObject.Table et, TMS e) {
		return gfasttm(LuaState.G(l), et, e);
	}

	public final static LuaConf.CharPtr[] luaT_typenames = { 
		LuaConf.CharPtr.toCharPtr("nil"), 
		LuaConf.CharPtr.toCharPtr("boolean"), 
		LuaConf.CharPtr.toCharPtr("userdata"), 
		LuaConf.CharPtr.toCharPtr("number"), 
		LuaConf.CharPtr.toCharPtr("string"), 
		LuaConf.CharPtr.toCharPtr("table"), 
		LuaConf.CharPtr.toCharPtr("function"), 
		LuaConf.CharPtr.toCharPtr("userdata"), 
		LuaConf.CharPtr.toCharPtr("thread"), 
		LuaConf.CharPtr.toCharPtr("proto"), 
		LuaConf.CharPtr.toCharPtr("upval") 
	};

	private final static LuaConf.CharPtr[] luaT_eventname = { 
		LuaConf.CharPtr.toCharPtr("__index"), 
		LuaConf.CharPtr.toCharPtr("__newindex"), 
		LuaConf.CharPtr.toCharPtr("__gc"), 
		LuaConf.CharPtr.toCharPtr("__mode"), 
		LuaConf.CharPtr.toCharPtr("__eq"), 
		LuaConf.CharPtr.toCharPtr("__add"), 
		LuaConf.CharPtr.toCharPtr("__sub"), 
		LuaConf.CharPtr.toCharPtr("__mul"), 
		LuaConf.CharPtr.toCharPtr("__div"), 
		LuaConf.CharPtr.toCharPtr("__mod"), 
		LuaConf.CharPtr.toCharPtr("__pow"), 
		LuaConf.CharPtr.toCharPtr("__unm"), 
		LuaConf.CharPtr.toCharPtr("__len"), 
		LuaConf.CharPtr.toCharPtr("__lt"), 
		LuaConf.CharPtr.toCharPtr("__le"), 
		LuaConf.CharPtr.toCharPtr("__concat"), 
		LuaConf.CharPtr.toCharPtr("__call") 
	};

	public static void luaT_init(LuaState.lua_State L) {
		int i;
		for (i = 0; i < TMS.TM_N.getValue(); i++) {
			LuaState.G(L).tmname[i] = LuaString.luaS_new(L, luaT_eventname[i]);
			LuaString.luaS_fix(LuaState.G(L).tmname[i]); // never collect these names 
		}
	}

//        
//		 ** function to be used with macro "fasttm": optimized for absence of
//		 ** tag methods
//		 
	public static TValue luaT_gettm(LuaObject.Table events, TMS event_, LuaObject.TString ename) {
		//const
		TValue tm = LuaTable.luaH_getstr(events, ename);
		LuaLimits.lua_assert(TMSUtil.convertTMStoInt(event_) <= TMSUtil.convertTMStoInt(TMS.TM_EQ));
		if (LuaObject.ttisnil(tm)) {
			// no tag method? 
			events.flags |= (byte)(1 << event_.getValue()); // cache this fact 
			return null;
		}
		else {
			return tm;
		}
	}

	public static TValue luaT_gettmbyobj(LuaState.lua_State L, TValue o, TMS event_) {
		LuaObject.Table mt;
		switch (LuaObject.ttype(o)) {
			case Lua.LUA_TTABLE: {
					mt = LuaObject.hvalue(o).metatable;
					break;
				}
			case Lua.LUA_TUSERDATA: {
					mt = LuaObject.uvalue(o).metatable;
					break;
				}
			default: {
					mt = LuaState.G(L).mt[LuaObject.ttype(o)];
					break;
				}
		}
		return ((mt != null) ? LuaTable.luaH_getstr(mt, LuaState.G(L).tmname[event_.getValue()]) : LuaObject.luaO_nilobject);
	}
}