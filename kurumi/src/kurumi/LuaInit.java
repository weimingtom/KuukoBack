package kurumi;

//
// ** $Id: linit.c,v 1.14.1.1 2007/12/27 13:02:25 roberto Exp $
// ** Initialization of libraries for lua.c
// ** See Copyright Notice in lua.h
// 
public class LuaInit {
	private final static LuaAuxLib.luaL_Reg[] lualibs = { 
		new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr(""), new LuaInit_delegate("LuaBaseLib.luaopen_base")), 
		new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr(LuaLib.LUA_LOADLIBNAME), new LuaInit_delegate("LuaLoadLib.luaopen_package")), 
		new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr(LuaLib.LUA_TABLIBNAME), new LuaInit_delegate("LuaTableLib.luaopen_table")), 
		new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr(LuaLib.LUA_IOLIBNAME), new LuaInit_delegate("LuaIOLib.luaopen_io")), 
		new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr(LuaLib.LUA_OSLIBNAME), new LuaInit_delegate("LuaOSLib.luaopen_os")), 
		new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr(LuaLib.LUA_STRLIBNAME), new LuaInit_delegate("LuaStrLib.luaopen_string")), 
		new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr(LuaLib.LUA_MATHLIBNAME), new LuaInit_delegate("LuaMathLib.luaopen_math")), 
		new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr(LuaLib.LUA_DBLIBNAME), new LuaInit_delegate("LuaDebugLib.luaopen_debug")), 
		new LuaAuxLib.luaL_Reg(null, null) 
	};

	public static class LuaInit_delegate implements Lua.lua_CFunction {
		private String name;

		public LuaInit_delegate(String name) {
			this.name = name;
		}

		public final int exec(LuaState.lua_State L) {
			if ((new String("LuaBaseLib.luaopen_base")).equals(name)) {
				return LuaBaseLib.luaopen_base(L);
			}
			else if ((new String("LuaLoadLib.luaopen_package")).equals(name)) {
				return LuaLoadLib.luaopen_package(L);
			}
			else if ((new String("LuaTableLib.luaopen_table")).equals(name)) {
				return LuaTableLib.luaopen_table(L);
			}
			else if ((new String("LuaIOLib.luaopen_io")).equals(name)) {
				return LuaIOLib.luaopen_io(L);
			}
			else if ((new String("LuaOSLib.luaopen_os")).equals(name)) {
				return LuaOSLib.luaopen_os(L);
			}
			else if ((new String("LuaStrLib.luaopen_string")).equals(name)) {
				return LuaStrLib.luaopen_string(L);
			}
			else if ((new String("LuaMathLib.luaopen_math")).equals(name)) {
				return LuaMathLib.luaopen_math(L);
			}
			else if ((new String("LuaDebugLib.luaopen_debug")).equals(name)) {
				return LuaDebugLib.luaopen_debug(L);
			}
			else {
				return 0;
			}
		}
	}


	public static void luaL_openlibs(LuaState.lua_State L) {
		for (int i = 0; i < lualibs.length - 1; i++) {
			LuaAuxLib.luaL_Reg lib = lualibs[i];
			Lua.lua_pushcfunction(L, lib.func);
			LuaAPI.lua_pushstring(L, lib.name);
			LuaAPI.lua_call(L, 1, 0);
		}
	}
}