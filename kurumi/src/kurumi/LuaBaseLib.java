package kurumi;

//
// ** $Id: lbaselib.c,v 1.191.1.6 2008/02/14 16:46:22 roberto Exp $
// ** Basic library
// ** See Copyright Notice in lua.h
// 

//using lua_Number = System.Double;

public class LuaBaseLib {
//        
//		 ** If your system does not support `stdout', you can just remove this function.
//		 ** If you need, you can define your own `print' function, following this
//		 ** model but changing `fputs' to put the strings at a proper place
//		 ** (a console window or a log file, for instance).
//		 
	private static int luaB_print(LuaState.lua_State L) {
		int n = LuaAPI.lua_gettop(L); // number of arguments 
		int i;
		Lua.lua_getglobal(L, CLib.CharPtr.toCharPtr("tostring"));
		for (i = 1; i <= n; i++) {
			CLib.CharPtr s;
			LuaAPI.lua_pushvalue(L, -1); // function to be called 
			LuaAPI.lua_pushvalue(L, i); // value to print 
			LuaAPI.lua_call(L, 1, 1);
			s = Lua.lua_tostring(L, -1); // get result 
			if (CLib.CharPtr.isEqual(s, null)) {
				return LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr(LuaConf.LUA_QL("tostring") + " must return a string to " + LuaConf.LUA_QL("print"))); //FIXME:
			}
			if (i > 1) {
				CLib.fputs(CLib.CharPtr.toCharPtr("\t"), CLib.stdout);
			}
			CLib.fputs(s, CLib.stdout);
			Lua.lua_pop(L, 1); // pop result 
		}
		//FIXME:
		//Console.Write("\n", CLib.stdout);
		StreamProxy.Write("\n");
		return 0;
	}

	private static int luaB_tonumber(LuaState.lua_State L) {
		int base_ = LuaAuxLib.luaL_optint(L, 2, 10);
		if (base_ == 10) {
			// standard conversion 
			LuaAuxLib.luaL_checkany(L, 1);
			if (LuaAPI.lua_isnumber(L, 1) != 0) {
				LuaAPI.lua_pushnumber(L, LuaAPI.lua_tonumber(L, 1));
				return 1;
			}
		}
		else {
			CLib.CharPtr s1 = LuaAuxLib.luaL_checkstring(L, 1);
			CLib.CharPtr[] s2 = new CLib.CharPtr[1];
			s2[0] = new CLib.CharPtr();
			long n; //ulong
			LuaAuxLib.luaL_argcheck(L, 2 <= base_ && base_ <= 36, 2, "base out of range");
			n = CLib.strtoul(s1, s2, base_); //out
			if (CLib.CharPtr.isNotEqual(s1, s2[0])) {
				// at least one valid digit? 
				while (CLib.isspace((byte)(s2[0].get(0)))) {
					s2[0] = s2[0].next(); // skip trailing spaces 
				}
				if (s2[0].get(0) == '\0') {
					// no invalid trailing characters? 
					LuaAPI.lua_pushnumber(L, (double)n); //lua_Number
					return 1;
				}
			}
		}
		LuaAPI.lua_pushnil(L); // else not a number 
		return 1;
	}

	private static int luaB_error(LuaState.lua_State L) {
		int level = LuaAuxLib.luaL_optint(L, 2, 1);
		LuaAPI.lua_settop(L, 1);
		if ((LuaAPI.lua_isstring(L, 1) != 0) && (level > 0)) {
			// add extra information? 
			LuaAuxLib.luaL_where(L, level);
			LuaAPI.lua_pushvalue(L, 1);
			LuaAPI.lua_concat(L, 2);
		}
		return LuaAPI.lua_error(L);
	}

	private static int luaB_getmetatable(LuaState.lua_State L) {
		LuaAuxLib.luaL_checkany(L, 1);
		if (LuaAPI.lua_getmetatable(L, 1) == 0) {
			LuaAPI.lua_pushnil(L);
			return 1; // no metatable 
		}
		LuaAuxLib.luaL_getmetafield(L, 1, CLib.CharPtr.toCharPtr("__metatable"));
		return 1; // returns either __metatable field (if present) or metatable 
	}

	private static int luaB_setmetatable(LuaState.lua_State L) {
		int t = LuaAPI.lua_type(L, 2);
		LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
		LuaAuxLib.luaL_argcheck(L, t == Lua.LUA_TNIL || t == Lua.LUA_TTABLE, 2, "nil or table expected");
		if (LuaAuxLib.luaL_getmetafield(L, 1, CLib.CharPtr.toCharPtr("__metatable")) != 0) {
			LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("cannot change a protected metatable"));
		}
		LuaAPI.lua_settop(L, 2);
		LuaAPI.lua_setmetatable(L, 1);
		return 1;
	}

	private static void getfunc(LuaState.lua_State L, int opt) {
		if (Lua.lua_isfunction(L, 1)) {
			LuaAPI.lua_pushvalue(L, 1);
		}
		else {
			Lua.lua_Debug ar = new Lua.lua_Debug();
			int level = (opt != 0) ? LuaAuxLib.luaL_optint(L, 1, 1) : LuaAuxLib.luaL_checkint(L, 1);
			LuaAuxLib.luaL_argcheck(L, level >= 0, 1, "level must be non-negative");
			if (LuaDebug.lua_getstack(L, level, ar) == 0) {
				LuaAuxLib.luaL_argerror(L, 1, CLib.CharPtr.toCharPtr("invalid level"));
			}
			LuaDebug.lua_getinfo(L, CLib.CharPtr.toCharPtr("f"), ar);
			if (Lua.lua_isnil(L, -1)) {
				LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("no function environment for tail call at level %d"), level);
			}
		}
	}

	private static int luaB_getfenv(LuaState.lua_State L) {
		getfunc(L, 1);
		if (LuaAPI.lua_iscfunction(L, -1)) { // is a C function? 
			LuaAPI.lua_pushvalue(L, Lua.LUA_GLOBALSINDEX); // return the thread's global env. 
		}
		else {
			LuaAPI.lua_getfenv(L, -1);
		}
		return 1;
	}

	private static int luaB_setfenv(LuaState.lua_State L) {
		LuaAuxLib.luaL_checktype(L, 2, Lua.LUA_TTABLE);
		getfunc(L, 0);
		LuaAPI.lua_pushvalue(L, 2);
		if ((LuaAPI.lua_isnumber(L, 1) != 0) && (LuaAPI.lua_tonumber(L, 1) == 0)) {
			// change environment of current thread 
			LuaAPI.lua_pushthread(L);
			LuaAPI.lua_insert(L, -2);
			LuaAPI.lua_setfenv(L, -2);
			return 0;
		}
		else if (LuaAPI.lua_iscfunction(L, -2) || LuaAPI.lua_setfenv(L, -2) == 0) {
			LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr(LuaConf.LUA_QL("setfenv") + " cannot change environment of given object"));
		}
		return 1;
	}

	private static int luaB_rawequal(LuaState.lua_State L) {
		LuaAuxLib.luaL_checkany(L, 1);
		LuaAuxLib.luaL_checkany(L, 2);
		LuaAPI.lua_pushboolean(L, LuaAPI.lua_rawequal(L, 1, 2));
		return 1;
	}

	private static int luaB_rawget(LuaState.lua_State L) {
		LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
		LuaAuxLib.luaL_checkany(L, 2);
		LuaAPI.lua_settop(L, 2);
		LuaAPI.lua_rawget(L, 1);
		return 1;
	}

	private static int luaB_rawset(LuaState.lua_State L) {
		LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
		LuaAuxLib.luaL_checkany(L, 2);
		LuaAuxLib.luaL_checkany(L, 3);
		LuaAPI.lua_settop(L, 3);
		LuaAPI.lua_rawset(L, 1);
		return 1;
	}

	private static int luaB_gcinfo(LuaState.lua_State L) {
		LuaAPI.lua_pushinteger(L, Lua.lua_getgccount(L));
		return 1;
	}

	public static final CLib.CharPtr[] opts = { 
		CLib.CharPtr.toCharPtr("stop"), 
		CLib.CharPtr.toCharPtr("restart"), 
		CLib.CharPtr.toCharPtr("collect"), 
		CLib.CharPtr.toCharPtr("count"), 
		CLib.CharPtr.toCharPtr("step"), 
		CLib.CharPtr.toCharPtr("setpause"), 
		CLib.CharPtr.toCharPtr("setstepmul"), 
		null 
	};

	public final static int[] optsnum = { 
		Lua.LUA_GCSTOP, 
		Lua.LUA_GCRESTART, 
		Lua.LUA_GCCOLLECT, 
		Lua.LUA_GCCOUNT, 
		Lua.LUA_GCSTEP, 
		Lua.LUA_GCSETPAUSE, 
		Lua.LUA_GCSETSTEPMUL 
	};

	private static int luaB_collectgarbage(LuaState.lua_State L) {
		int o = LuaAuxLib.luaL_checkoption(L, 1, CLib.CharPtr.toCharPtr("collect"), opts);
		int ex = LuaAuxLib.luaL_optint(L, 2, 0);
		int res = LuaAPI.lua_gc(L, optsnum[o], ex);
		switch (optsnum[o]) {
			case Lua.LUA_GCCOUNT: {
					int b = LuaAPI.lua_gc(L, Lua.LUA_GCCOUNTB, 0);
					LuaAPI.lua_pushnumber(L, res + ((double)b / 1024)); //lua_Number
					return 1;
				}
			case Lua.LUA_GCSTEP: {
					LuaAPI.lua_pushboolean(L, res);
					return 1;
				}
				default: {
					LuaAPI.lua_pushnumber(L, res);
					return 1;
				}
		}
	}

	private static int luaB_type(LuaState.lua_State L) {
		LuaAuxLib.luaL_checkany(L, 1);
		LuaAPI.lua_pushstring(L, LuaAuxLib.luaL_typename(L, 1));
		return 1;
	}

	private static int luaB_next(LuaState.lua_State L) {
		LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
		LuaAPI.lua_settop(L, 2); // create a 2nd argument if there isn't one 
		if (LuaAPI.lua_next(L, 1) != 0) {
			return 2;
		}
		else {
			LuaAPI.lua_pushnil(L);
			return 1;
		}
	}

	private static int luaB_pairs(LuaState.lua_State L) {
		LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
		LuaAPI.lua_pushvalue(L, Lua.lua_upvalueindex(1)); // return generator, 
		LuaAPI.lua_pushvalue(L, 1); // state, 
		LuaAPI.lua_pushnil(L); // and initial value 
		return 3;
	}

	private static int ipairsaux(LuaState.lua_State L) {
		int i = LuaAuxLib.luaL_checkint(L, 2);
		LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
		i++; // next value 
		LuaAPI.lua_pushinteger(L, i);
		LuaAPI.lua_rawgeti(L, 1, i);
		return (Lua.lua_isnil(L, -1)) ? 0 : 2;
	}

	private static int luaB_ipairs(LuaState.lua_State L) {
		LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
		LuaAPI.lua_pushvalue(L, Lua.lua_upvalueindex(1)); // return generator, 
		LuaAPI.lua_pushvalue(L, 1); // state, 
		LuaAPI.lua_pushinteger(L, 0); // and initial value 
		return 3;
	}

	private static int load_aux(LuaState.lua_State L, int status) {
		if (status == 0) { // OK? 
			return 1;
		}
		else {
			LuaAPI.lua_pushnil(L);
			LuaAPI.lua_insert(L, -2); // put before error message 
			return 2; // return nil plus error message 
		}
	}

	private static int luaB_loadstring(LuaState.lua_State L) {
		int[] l = new int[1]; //uint
		CLib.CharPtr s = LuaAuxLib.luaL_checklstring(L, 1, l); //out
		CLib.CharPtr chunkname = LuaAuxLib.luaL_optstring(L, 2, s);
		return load_aux(L, LuaAuxLib.luaL_loadbuffer(L, s, l[0], chunkname));
	}

	private static int luaB_loadfile(LuaState.lua_State L) {
		CLib.CharPtr fname = LuaAuxLib.luaL_optstring(L, 1, null);
		return load_aux(L, LuaAuxLib.luaL_loadfile(L, fname));
	}

//        
//		 ** Reader for generic `load' function: `lua_load' uses the
//		 ** stack for internal stuff, so the reader cannot change the
//		 ** stack top. Instead, it keeps its resulting string in a
//		 ** reserved slot inside the stack.
//		 
	private static CLib.CharPtr generic_reader(LuaState.lua_State L, Object ud, int[] size) { //uint - out
		//(void)ud;  /* to avoid warnings */
		LuaAuxLib.luaL_checkstack(L, 2, CLib.CharPtr.toCharPtr("too many nested functions"));
		LuaAPI.lua_pushvalue(L, 1); // get function 
		LuaAPI.lua_call(L, 0, 1); // call it 
		if (Lua.lua_isnil(L, -1)) {
			size[0] = 0;
			return null;
		}
		else if (LuaAPI.lua_isstring(L, -1) != 0) {
			LuaAPI.lua_replace(L, 3); // save string in a reserved stack slot 
			return LuaAPI.lua_tolstring(L, 3, size); //out
		}
		else {
			size[0] = 0;
			LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("reader function must return a string"));
		}
		return null; // to avoid warnings 
	}

	public static class generic_reader_delegate implements Lua.lua_Reader {
		public final CLib.CharPtr exec(LuaState.lua_State L, Object ud, int[] sz) { //uint - out
			return generic_reader(L, ud, sz); //out
		}
	}


	private static int luaB_load(LuaState.lua_State L) {
		int status;
		CLib.CharPtr cname = LuaAuxLib.luaL_optstring(L, 2, CLib.CharPtr.toCharPtr("=(load)"));
		LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TFUNCTION);
		LuaAPI.lua_settop(L, 3); // function, eventual name, plus one reserved slot 
		status = LuaAPI.lua_load(L, new generic_reader_delegate(), null, cname);
		return load_aux(L, status);
	}

	private static int luaB_dofile(LuaState.lua_State L) {
		CLib.CharPtr fname = LuaAuxLib.luaL_optstring(L, 1, null);
		int n = LuaAPI.lua_gettop(L);
		if (LuaAuxLib.luaL_loadfile(L, fname) != 0) {
			LuaAPI.lua_error(L);
		}
		LuaAPI.lua_call(L, 0, Lua.LUA_MULTRET);
		return LuaAPI.lua_gettop(L) - n;
	}

	private static int luaB_assert(LuaState.lua_State L) {
		LuaAuxLib.luaL_checkany(L, 1);
		if (LuaAPI.lua_toboolean(L, 1) == 0) {
			return LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("%s"), LuaAuxLib.luaL_optstring(L, 2, CLib.CharPtr.toCharPtr("assertion failed!")));
		}
		return LuaAPI.lua_gettop(L);
	}

	private static int luaB_unpack(LuaState.lua_State L) {
		int i, e, n;
		LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TTABLE);
		i = LuaAuxLib.luaL_optint(L, 2, 1);
		e = LuaAuxLib.luaL_opt_integer(L, new LuaAuxLib.luaL_checkint_delegate(), 3, LuaAuxLib.luaL_getn(L, 1));
		if (i > e) {
			return 0; // empty range 
		}
		n = e - i + 1; // number of elements 
		if (n <= 0 || (LuaAPI.lua_checkstack(L, n) == 0)) { // n <= 0 means arith. overflow 
			return LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("too many results to unpack"));
		}
		LuaAPI.lua_rawgeti(L, 1, i); // push arg[i] (avoiding overflow problems) 
		while (i++ < e) { // push arg[i + 1...e] 
			LuaAPI.lua_rawgeti(L, 1, i);
		}
		return n;
	}

	private static int luaB_select(LuaState.lua_State L) {
		int n = LuaAPI.lua_gettop(L);
		if (LuaAPI.lua_type(L, 1) == Lua.LUA_TSTRING && Lua.lua_tostring(L, 1).get(0) == '#') {
			LuaAPI.lua_pushinteger(L, n - 1);
			return 1;
		}
		else {
			int i = LuaAuxLib.luaL_checkint(L, 1);
			if (i < 0) {
				i = n + i;
			}
			else if (i > n) {
				i = n;
			}
			LuaAuxLib.luaL_argcheck(L, 1 <= i, 1, "index out of range");
			return n - i;
		}
	}

	private static int luaB_pcall(LuaState.lua_State L) {
		int status;
		LuaAuxLib.luaL_checkany(L, 1);
		status = LuaAPI.lua_pcall(L, LuaAPI.lua_gettop(L) - 1, Lua.LUA_MULTRET, 0);
		LuaAPI.lua_pushboolean(L, (status == 0) ? 1 : 0);
		LuaAPI.lua_insert(L, 1);
		return LuaAPI.lua_gettop(L); // return status + all results 
	}

	private static int luaB_xpcall(LuaState.lua_State L) {
		int status;
		LuaAuxLib.luaL_checkany(L, 2);
		LuaAPI.lua_settop(L, 2);
		LuaAPI.lua_insert(L, 1); // put error function under function to be called 
		status = LuaAPI.lua_pcall(L, 0, Lua.LUA_MULTRET, 1);
		LuaAPI.lua_pushboolean(L, (status == 0) ? 1 : 0);
		LuaAPI.lua_replace(L, 1);
		return LuaAPI.lua_gettop(L); // return status + all results 
	}

	private static int luaB_tostring(LuaState.lua_State L) {
		LuaAuxLib.luaL_checkany(L, 1);
		if (LuaAuxLib.luaL_callmeta(L, 1, CLib.CharPtr.toCharPtr("__tostring")) != 0) { // is there a metafield? 
			return 1; // use its value 
		}
		switch (LuaAPI.lua_type(L, 1)) {
			case Lua.LUA_TNUMBER: {
					LuaAPI.lua_pushstring(L, Lua.lua_tostring(L, 1));
					break;
				}
			case Lua.LUA_TSTRING: {
					LuaAPI.lua_pushvalue(L, 1);
					break;
				}
			case Lua.LUA_TBOOLEAN: {
					LuaAPI.lua_pushstring(L, (LuaAPI.lua_toboolean(L, 1) != 0 ? CLib.CharPtr.toCharPtr("true") : CLib.CharPtr.toCharPtr("false")));
					break;
				}
			case Lua.LUA_TNIL: {
					Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr("nil"));
					break;
				}
			default: {
					LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr("%s: %p"), LuaAuxLib.luaL_typename(L, 1), LuaAPI.lua_topointer(L, 1));
					break;
				}
		}
		return 1;
	}

	private static int luaB_newproxy(LuaState.lua_State L) {
		LuaAPI.lua_settop(L, 1);
		LuaAPI.lua_newuserdata(L, 0); // create proxy 
		if (LuaAPI.lua_toboolean(L, 1) == 0) {
			return 1; // no metatable 
		}
		else if (Lua.lua_isboolean(L, 1)) {
			Lua.lua_newtable(L); // create a new metatable `m'... 
			LuaAPI.lua_pushvalue(L, -1); //... and mark `m' as a valid metatable 
			LuaAPI.lua_pushboolean(L, 1);
			LuaAPI.lua_rawset(L, Lua.lua_upvalueindex(1)); // weaktable[m] = true 
		}
		else {
			int validproxy = 0; // to check if weaktable[metatable(u)] == true 
			if (LuaAPI.lua_getmetatable(L, 1) != 0) {
				LuaAPI.lua_rawget(L, Lua.lua_upvalueindex(1));
				validproxy = LuaAPI.lua_toboolean(L, -1);
				Lua.lua_pop(L, 1); // remove value 
			}
			LuaAuxLib.luaL_argcheck(L, validproxy != 0, 1, "boolean or proxy expected");
			LuaAPI.lua_getmetatable(L, 1); // metatable is valid; get it 
		}
		LuaAPI.lua_setmetatable(L, 2);
		return 1;
	}


	private final static LuaAuxLib.luaL_Reg[] base_funcs = { 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("assert"), new LuaBaseLib_delegate("luaB_assert")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("collectgarbage"), new LuaBaseLib_delegate("luaB_collectgarbage")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("dofile"), new LuaBaseLib_delegate("luaB_dofile")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("error"), new LuaBaseLib_delegate("luaB_error")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("gcinfo"), new LuaBaseLib_delegate("luaB_gcinfo")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("getfenv"), new LuaBaseLib_delegate("luaB_getfenv")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("getmetatable"), new LuaBaseLib_delegate("luaB_getmetatable")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("loadfile"), new LuaBaseLib_delegate("luaB_loadfile")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("load"), new LuaBaseLib_delegate("luaB_load")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("loadstring"), new LuaBaseLib_delegate("luaB_loadstring")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("next"), new LuaBaseLib_delegate("luaB_next")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("pcall"), new LuaBaseLib_delegate("luaB_pcall")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("print"), new LuaBaseLib_delegate("luaB_print")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("rawequal"), new LuaBaseLib_delegate("luaB_rawequal")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("rawget"), new LuaBaseLib_delegate("luaB_rawget")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("rawset"), new LuaBaseLib_delegate("luaB_rawset")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("select"), new LuaBaseLib_delegate("luaB_select")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("setfenv"), new LuaBaseLib_delegate("luaB_setfenv")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("setmetatable"), new LuaBaseLib_delegate("luaB_setmetatable")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("tonumber"), new LuaBaseLib_delegate("luaB_tonumber")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("tostring"), new LuaBaseLib_delegate("luaB_tostring")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("type"), new LuaBaseLib_delegate("luaB_type")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("unpack"), new LuaBaseLib_delegate("luaB_unpack")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("xpcall"), new LuaBaseLib_delegate("luaB_xpcall")), 
		new LuaAuxLib.luaL_Reg(null, null) 
	};

	public static class LuaBaseLib_delegate implements Lua.lua_CFunction {
		private String name;

		public LuaBaseLib_delegate(String name) {
			this.name = name;
		}

		public final int exec(LuaState.lua_State L) {
			if ((new String("luaB_assert")).equals(name)) {
				return luaB_assert(L);
			}
			else if ((new String("luaB_collectgarbage")).equals(name)) {
				return luaB_collectgarbage(L);
			}
			else if ((new String("luaB_dofile")).equals(name)) {
				return luaB_dofile(L);
			}
			else if ((new String("luaB_error")).equals(name)) {
				return luaB_error(L);
			}
			else if ((new String("luaB_gcinfo")).equals(name)) {
				return luaB_gcinfo(L);
			}
			else if ((new String("luaB_getfenv")).equals(name)) {
				return luaB_getfenv(L);
			}
			else if ((new String("luaB_getmetatable")).equals(name)) {
				return luaB_getmetatable(L);
			}
			else if ((new String("luaB_loadfile")).equals(name)) {
				return luaB_loadfile(L);
			}
			else if ((new String("luaB_load")).equals(name)) {
				return luaB_load(L);
			}
			else if ((new String("luaB_loadstring")).equals(name)) {
				return luaB_loadstring(L);
			}
			else if ((new String("luaB_next")).equals(name)) {
				return luaB_next(L);
			}
			else if ((new String("luaB_pcall")).equals(name)) {
				return luaB_pcall(L);
			}
			else if ((new String("luaB_print")).equals(name)) {
				return luaB_print(L);
			}
			else if ((new String("luaB_rawequal")).equals(name)) {
				return luaB_rawequal(L);
			}
			else if ((new String("luaB_rawget")).equals(name)) {
				return luaB_rawget(L);
			}
			else if ((new String("luaB_rawset")).equals(name)) {
				return luaB_rawset(L);
			}
			else if ((new String("luaB_select")).equals(name)) {
				return luaB_select(L);
			}
			else if ((new String("luaB_setfenv")).equals(name)) {
				return luaB_setfenv(L);
			}
			else if ((new String("luaB_setmetatable")).equals(name)) {
				return luaB_setmetatable(L);
			}
			else if ((new String("luaB_tonumber")).equals(name)) {
				return luaB_tonumber(L);
			}
			else if ((new String("luaB_tostring")).equals(name)) {
				return luaB_tostring(L);
			}
			else if ((new String("luaB_type")).equals(name)) {
				return luaB_type(L);
			}
			else if ((new String("luaB_unpack")).equals(name)) {
				return luaB_unpack(L);
			}
			else if ((new String("luaB_xpcall")).equals(name)) {
				return luaB_xpcall(L);
			}
			if ((new String("luaB_cocreate")).equals(name)) {
				return luaB_cocreate(L);
			}
			else if ((new String("luaB_coresume")).equals(name)) {
				return luaB_coresume(L);
			}
			else if ((new String("luaB_corunning")).equals(name)) {
				return luaB_corunning(L);
			}
			else if ((new String("luaB_costatus")).equals(name)) {
				return luaB_costatus(L);
			}
			else if ((new String("luaB_cowrap")).equals(name)) {
				return luaB_cowrap(L);
			}
			else if ((new String("luaB_yield")).equals(name)) {
				return luaB_yield(L);
			}
			else if ((new String("luaB_ipairs")).equals(name)) {
				return luaB_ipairs(L);
			}
			else if ((new String("ipairsaux")).equals(name)) {
				return ipairsaux(L);
			}
			else if ((new String("luaB_pairs")).equals(name)) {
				return luaB_pairs(L);
			}
			else if ((new String("luaB_next")).equals(name)) {
				return luaB_next(L);
			}
			else if ((new String("luaB_newproxy")).equals(name)) {
				return luaB_newproxy(L);
			}
			else if ((new String("luaB_auxwrap")).equals(name)) {
				return luaB_auxwrap(L);
			}
			else {
				return 0;
			}
		}
	}


//        
//		 ** {======================================================
//		 ** Coroutine library
//		 ** =======================================================
//		 

	public static final int CO_RUN = 0; // running 
	public static final int CO_SUS = 1; // suspended 
	public static final int CO_NOR = 2; // 'normal' (it resumed another coroutine) 
	public static final int CO_DEAD = 3;

	private static final String[] statnames = { "running", "suspended", "normal", "dead" };

	private static int costatus(LuaState.lua_State L, LuaState.lua_State co) {
		if (L == co) {
			return CO_RUN;
		}
		switch (LuaAPI.lua_status(co)) {
			case Lua.LUA_YIELD: {
					return CO_SUS;
				}
			case 0: {
					Lua.lua_Debug ar = new Lua.lua_Debug();
					if (LuaDebug.lua_getstack(co, 0, ar) > 0) { // does it have frames? 
						return CO_NOR; // it is running 
					}
					else if (LuaAPI.lua_gettop(co) == 0) {
						return CO_DEAD;
					}
					else {
						return CO_SUS; // initial state 
					}
				}
			default: { // some error occured 
					return CO_DEAD;
				}
		}
	}

	private static int luaB_costatus(LuaState.lua_State L) {
		LuaState.lua_State co = LuaAPI.lua_tothread(L, 1);
		LuaAuxLib.luaL_argcheck(L, co != null, 1, "coroutine expected");
		LuaAPI.lua_pushstring(L, CLib.CharPtr.toCharPtr(statnames[costatus(L, co)]));
		return 1;
	}

	private static int auxresume(LuaState.lua_State L, LuaState.lua_State co, int narg) {
		int status = costatus(L, co);
		if (LuaAPI.lua_checkstack(co, narg) == 0) {
			LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("too many arguments to resume"));
		}
		if (status != CO_SUS) {
			LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr("cannot resume %s coroutine"), statnames[status]);
			return -1; // error flag 
		}
		LuaAPI.lua_xmove(L, co, narg);
		LuaAPI.lua_setlevel(L, co);
		status = LuaDo.lua_resume(co, narg);
		if (status == 0 || status == Lua.LUA_YIELD) {
			int nres = LuaAPI.lua_gettop(co);
			if (LuaAPI.lua_checkstack(L, nres + 1) == 0) {
				LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("too many results to resume"));
			}
			LuaAPI.lua_xmove(co, L, nres); // move yielded values 
			return nres;
		}
		else {
			LuaAPI.lua_xmove(co, L, 1); // move error message 
			return -1; // error flag 
		}
	}

	private static int luaB_coresume(LuaState.lua_State L) {
		LuaState.lua_State co = LuaAPI.lua_tothread(L, 1);
		int r;
		LuaAuxLib.luaL_argcheck(L, co != null, 1, "coroutine expected");
		r = auxresume(L, co, LuaAPI.lua_gettop(L) - 1);
		if (r < 0) {
			LuaAPI.lua_pushboolean(L, 0);
			LuaAPI.lua_insert(L, -2);
			return 2; // return false + error message 
		}
		else {
			LuaAPI.lua_pushboolean(L, 1);
			LuaAPI.lua_insert(L, -(r + 1));
			return r + 1; // return true + `resume' returns 
		}
	}

	private static int luaB_auxwrap(LuaState.lua_State L) {
		LuaState.lua_State co = LuaAPI.lua_tothread(L, Lua.lua_upvalueindex(1));
		int r = auxresume(L, co, LuaAPI.lua_gettop(L));
		if (r < 0) {
			if (LuaAPI.lua_isstring(L, -1) != 0) {
				// error object is a string? 
				LuaAuxLib.luaL_where(L, 1); // add extra info 
				LuaAPI.lua_insert(L, -2);
				LuaAPI.lua_concat(L, 2);
			}
			LuaAPI.lua_error(L); // propagate error 
		}
		return r;
	}

	private static int luaB_cocreate(LuaState.lua_State L) {
		LuaState.lua_State NL = LuaAPI.lua_newthread(L);
		LuaAuxLib.luaL_argcheck(L, Lua.lua_isfunction(L, 1) && !LuaAPI.lua_iscfunction(L, 1), 1, "Lua function expected");
		LuaAPI.lua_pushvalue(L, 1); // move function to top 
		LuaAPI.lua_xmove(L, NL, 1); // move function from L to NL 
		return 1;
	}


	private static int luaB_cowrap(LuaState.lua_State L) {
		luaB_cocreate(L);
		LuaAPI.lua_pushcclosure(L, new LuaBaseLib_delegate("luaB_auxwrap"), 1);
		return 1;
	}

	private static int luaB_yield(LuaState.lua_State L) {
		return LuaDo.lua_yield(L, LuaAPI.lua_gettop(L));
	}

	private static int luaB_corunning(LuaState.lua_State L) {
		if (LuaAPI.lua_pushthread(L) != 0) {
			LuaAPI.lua_pushnil(L); // main thread is not a coroutine 
		}
		return 1;
	}

	private final static LuaAuxLib.luaL_Reg[] co_funcs = { 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("create"), new LuaBaseLib_delegate("luaB_cocreate")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("resume"), new LuaBaseLib_delegate("luaB_coresume")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("running"), new LuaBaseLib_delegate("luaB_corunning")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("status"), new LuaBaseLib_delegate("luaB_costatus")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("wrap"), new LuaBaseLib_delegate("luaB_cowrap")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("yield"), new LuaBaseLib_delegate("luaB_yield")), 
		new LuaAuxLib.luaL_Reg(null, null) 
	};



	// }====================================================== 

	private static void auxopen(LuaState.lua_State L, CLib.CharPtr name, Lua.lua_CFunction f, Lua.lua_CFunction u) {
		Lua.lua_pushcfunction(L, u);
		LuaAPI.lua_pushcclosure(L, f, 1);
		LuaAPI.lua_setfield(L, -2, name);
	}

	private static void base_open(LuaState.lua_State L) {
		// set global _G 
		LuaAPI.lua_pushvalue(L, Lua.LUA_GLOBALSINDEX);
		Lua.lua_setglobal(L, CLib.CharPtr.toCharPtr("_G"));
		// open lib into global table 
		LuaAuxLib.luaL_register(L, CLib.CharPtr.toCharPtr("_G"), base_funcs);
		Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr(Lua.LUA_VERSION));
		Lua.lua_setglobal(L, CLib.CharPtr.toCharPtr("_VERSION")); // set global _VERSION 
		// `ipairs' and `pairs' need auxliliary functions as upvalues 
		auxopen(L, CLib.CharPtr.toCharPtr("ipairs"), new LuaBaseLib_delegate("luaB_ipairs"), new LuaBaseLib_delegate("ipairsaux"));
		auxopen(L, CLib.CharPtr.toCharPtr("pairs"), new LuaBaseLib_delegate("luaB_pairs"), new LuaBaseLib_delegate("luaB_next"));
		// `newproxy' needs a weaktable as upvalue 
		LuaAPI.lua_createtable(L, 0, 1); // new table `w' 
		LuaAPI.lua_pushvalue(L, -1); // `w' will be its own metatable 
		LuaAPI.lua_setmetatable(L, -2);
		Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr("kv"));
		LuaAPI.lua_setfield(L, -2, CLib.CharPtr.toCharPtr("__mode")); // metatable(w).__mode = "kv" 
		LuaAPI.lua_pushcclosure(L, new LuaBaseLib_delegate("luaB_newproxy"), 1);
		Lua.lua_setglobal(L, CLib.CharPtr.toCharPtr("newproxy")); // set global `newproxy' 
	}

	public static int luaopen_base(LuaState.lua_State L) {
		base_open(L);
		LuaAuxLib.luaL_register(L, CLib.CharPtr.toCharPtr(LuaLib.LUA_COLIBNAME), co_funcs);
		return 2;
	}
}