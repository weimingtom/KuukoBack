package kurumi;

//
// ** $Id: loslib.c,v 1.19.1.3 2008/01/18 16:38:18 roberto Exp $
// ** Standard Operating System library
// ** See Copyright Notice in lua.h
// 

//using TValue = Lua.TValue;
//using StkId = TValue;
//using lua_Integer = System.Int32;
//using lua_Number = System.Double;

public class LuaOSLib {
	private static int os_pushresult(LuaState.lua_State L, int i, CLib.CharPtr filename) {
		int en = CLib.errno(); // calls to Lua API may change this value 
		if (i != 0) {
			LuaAPI.lua_pushboolean(L, 1);
			return 1;
		}
		else {
			LuaAPI.lua_pushnil(L);
			LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr("%s: %s"), filename, CLib.strerror(en));
			LuaAPI.lua_pushinteger(L, en);
			return 3;
		}
	}

	private static int os_execute(LuaState.lua_State L) {
		CLib.CharPtr strCmdLine = CLib.CharPtr.toCharPtr("" + LuaAuxLib.luaL_optstring(L, 1, null));
		LuaAPI.lua_pushinteger(L, ClassType.processExec(strCmdLine.toString()));
		return 1;
	}

	private static int os_remove(LuaState.lua_State L) {
		CLib.CharPtr filename = LuaAuxLib.luaL_checkstring(L, 1);
		int result = 1;
		try {
			StreamProxy.Delete(filename.toString());
		}
		catch (java.lang.Exception e) {
			result = 0;
		}
		return os_pushresult(L, result, filename);
	}

	private static int os_rename(LuaState.lua_State L) {
		CLib.CharPtr fromname = LuaAuxLib.luaL_checkstring(L, 1);
		CLib.CharPtr toname = LuaAuxLib.luaL_checkstring(L, 2);
		int result;
		try {
			StreamProxy.Move(fromname.toString(), toname.toString());
			result = 0;
		}
		catch (java.lang.Exception e) {
			result = 1; // todo: this should be a proper error code
		}
		return os_pushresult(L, result, fromname);
	}

	private static int os_tmpname(LuaState.lua_State L) {
		LuaAPI.lua_pushstring(L, CLib.CharPtr.toCharPtr(StreamProxy.GetTempFileName()));
		return 1;
	}


	private static int os_getenv(LuaState.lua_State L) {
		LuaAPI.lua_pushstring(L, CLib.getenv(LuaAuxLib.luaL_checkstring(L, 1))); // if null push nil 
		return 1;
	}

	private static int os_clock(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, DateTimeProxy.getClock());
		return 1;
	}

//        
//		 ** {======================================================
//		 ** Time/Date operations
//		 ** { year=%Y, month=%m, day=%d, hour=%H, min=%M, sec=%S,
//		 **   wday=%w+1, yday=%j, isdst=? }
//		 ** =======================================================
//		 
	private static void setfield(LuaState.lua_State L, CLib.CharPtr key, int value) {
		LuaAPI.lua_pushinteger(L, value);
		LuaAPI.lua_setfield(L, -2, key);
	}

	private static void setboolfield(LuaState.lua_State L, CLib.CharPtr key, int value) {
		if (value < 0) { // undefined? 
			return; // does not set field 
		}
		LuaAPI.lua_pushboolean(L, value);
		LuaAPI.lua_setfield(L, -2, key);
	}

	private static int getboolfield(LuaState.lua_State L, CLib.CharPtr key) {
		int res;
		LuaAPI.lua_getfield(L, -1, key);
		res = Lua.lua_isnil(L, -1) ? -1 : LuaAPI.lua_toboolean(L, -1);
		Lua.lua_pop(L, 1);
		return res;
	}

	private static int getfield(LuaState.lua_State L, CLib.CharPtr key, int d) {
		int res;
		LuaAPI.lua_getfield(L, -1, key);
		if (LuaAPI.lua_isnumber(L, -1) != 0) {
			res = (int)LuaAPI.lua_tointeger(L, -1);
		}
		else {
			if (d < 0) {
				return LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("field " + LuaConf.getLUA_QS() + " missing in date table"), key);
			}
			res = d;
		}
		Lua.lua_pop(L, 1);
		return res;
	}

	private static int os_date(LuaState.lua_State L) {
		CLib.CharPtr s = LuaAuxLib.luaL_optstring(L, 1, CLib.CharPtr.toCharPtr("%c"));
		DateTimeProxy stm = new DateTimeProxy();
		if (s.get(0) == '!') {
			// UTC? 
			stm.setUTCNow();
			s.inc(); // skip `!' 
		}
		else {
			stm.setNow();
		}
		if (CLib.strcmp(s, CLib.CharPtr.toCharPtr("*t")) == 0) {
			LuaAPI.lua_createtable(L, 0, 9); // 9 = number of fields 
			setfield(L, CLib.CharPtr.toCharPtr("sec"), stm.getSecond());
			setfield(L, CLib.CharPtr.toCharPtr("min"), stm.getMinute());
			setfield(L, CLib.CharPtr.toCharPtr("hour"), stm.getHour());
			setfield(L, CLib.CharPtr.toCharPtr("day"), stm.getDay());
			setfield(L, CLib.CharPtr.toCharPtr("month"), stm.getMonth());
			setfield(L, CLib.CharPtr.toCharPtr("year"), stm.getYear());
			setfield(L, CLib.CharPtr.toCharPtr("wday"), (int)stm.getDayOfWeek());
			setfield(L, CLib.CharPtr.toCharPtr("yday"), stm.getDayOfYear());
			setboolfield(L, CLib.CharPtr.toCharPtr("isdst"), stm.IsDaylightSavingTime() ? 1 : 0);
		}
		else {
			//LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("strftime not implemented yet")); // todo: implement this - mjf
			///#if false
			CLib.CharPtr cc = CLib.CharPtr.toCharPtr(new char[3]);
			LuaAuxLib.luaL_Buffer b = new LuaAuxLib.luaL_Buffer();
			cc.set(0, '%'); cc.set(2, '\0');
			LuaAuxLib.luaL_buffinit(L, b);
			for (; s.get(0) != 0; s.inc())
			{
				if (s.get(0) != '%' || s.get(1) == '\0')  /* no conversion specifier? */
				{
					LuaAuxLib.luaL_addchar(b, s.get(0));
				}
				else 
				{
					int reslen;
					CLib.CharPtr buff = CLib.CharPtr.toCharPtr(new char[200]);  /* should be big enough for any conversion result */
					s.inc(); cc.set(1, s.get(0));
					reslen = CLib.strftime(buff, buff.chars.length, cc, stm); //string kkk = buff.ToString();
					LuaAuxLib.luaL_addlstring(b, buff, (int)reslen);
				}
			}
			LuaAuxLib.luaL_pushresult(b);
			///#endif // #if 0
		}
		return 1;
	}

	private static int os_time(LuaState.lua_State L) {
		DateTimeProxy t = new DateTimeProxy();
		if (Lua.lua_isnoneornil(L, 1)) { // called without args? 
			t.setNow(); // get current time 
		}
		else {
			LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
			LuaAPI.lua_settop(L, 1); // make sure table is at the top 
			int sec = getfield(L, CLib.CharPtr.toCharPtr("sec"), 0);
			int min = getfield(L, CLib.CharPtr.toCharPtr("min"), 0);
			int hour = getfield(L, CLib.CharPtr.toCharPtr("hour"), 12);
			int day = getfield(L, CLib.CharPtr.toCharPtr("day"), -1);
			int month = getfield(L, CLib.CharPtr.toCharPtr("month"), -1) - 1;
			int year = getfield(L, CLib.CharPtr.toCharPtr("year"), -1) - 1900;
			int isdst = getboolfield(L, CLib.CharPtr.toCharPtr("isdst")); // todo: implement this - mjf
			t = new DateTimeProxy(year, month, day, hour, min, sec);
		}
		LuaAPI.lua_pushnumber(L, t.getTicks());
		return 1;
	}

	private static int os_difftime(LuaState.lua_State L) {
		long ticks = (long)LuaAuxLib.luaL_checknumber(L, 1) - (long)LuaAuxLib.luaL_optnumber(L, 2, 0);
		LuaAPI.lua_pushnumber(L, ticks / 10000000); //FIXME: ticks / TimeSpan.TicksPerSecond
		return 1;
	}

	// }====================================================== 

	// locale not supported yet
	private static int os_setlocale(LuaState.lua_State L) {
//            
//		  static string[] cat = {LC_ALL, LC_COLLATE, LC_CTYPE, LC_MONETARY,
//							  LC_NUMERIC, LC_TIME};
//		  static string[] catnames[] = {"all", "collate", "ctype", "monetary",
//			 "numeric", "time", null};
//		  CharPtr l = luaL_optstring(L, 1, null);
//		  int op = luaL_checkoption(L, 2, "all", catnames);
//		  lua_pushstring(L, setlocale(cat[op], l));
//			 
		CLib.CharPtr l = LuaAuxLib.luaL_optstring(L, 1, null);
		LuaAPI.lua_pushstring(L, CLib.CharPtr.toCharPtr("C"));
		return (l.toString().equals("C")) ? 1 : 0;
	}

	private static int os_exit(LuaState.lua_State L) {
		System.exit(CLib.EXIT_SUCCESS);
		return 0;
	}

	private final static LuaAuxLib.luaL_Reg[] syslib = { 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("clock"), new LuaOSLib_delegate("os_clock")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("date"), new LuaOSLib_delegate("os_date")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("difftime"), new LuaOSLib_delegate("os_difftime")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("execute"), new LuaOSLib_delegate("os_execute")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("exit"), new LuaOSLib_delegate("os_exit")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("getenv"), new LuaOSLib_delegate("os_getenv")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("remove"), new LuaOSLib_delegate("os_remove")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("rename"), new LuaOSLib_delegate("os_rename")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("setlocale"), new LuaOSLib_delegate("os_setlocale")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("time"), new LuaOSLib_delegate("os_time")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("tmpname"), new LuaOSLib_delegate("os_tmpname")), 
		new LuaAuxLib.luaL_Reg(null, null) 
	};

	public static class LuaOSLib_delegate implements Lua.lua_CFunction {
		private String name;

		public LuaOSLib_delegate(String name) {
			this.name = name;
		}

		public final int exec(LuaState.lua_State L) {
			if ((new String("os_clock")).equals(name)) {
				return os_clock(L);
			}
			else if ((new String("os_date")).equals(name)) {
				return os_date(L);
			}
			else if ((new String("os_difftime")).equals(name)) {
				return os_difftime(L);
			}
			else if ((new String("os_execute")).equals(name)) {
				return os_execute(L);
			}
			else if ((new String("os_exit")).equals(name)) {
				return os_exit(L);
			}
			else if ((new String("os_getenv")).equals(name)) {
				return os_getenv(L);
			}
			else if ((new String("os_remove")).equals(name)) {
				return os_remove(L);
			}
			else if ((new String("os_rename")).equals(name)) {
				return os_rename(L);
			}
			else if ((new String("os_setlocale")).equals(name)) {
				return os_setlocale(L);
			}
			else if ((new String("os_time")).equals(name)) {
				return os_time(L);
			}
			else if ((new String("os_tmpname")).equals(name)) {
				return os_tmpname(L);
			}
			else {
				return 0;
			}
		}
	}


	// }====================================================== 

	public static int luaopen_os(LuaState.lua_State L) {
		LuaAuxLib.luaL_register(L, CLib.CharPtr.toCharPtr(LuaLib.LUA_OSLIBNAME), syslib);
		return 1;
	}
}