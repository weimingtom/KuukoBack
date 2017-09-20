/*
 ** $Id: lauxlib.c,v 1.159.1.3 2008/01/21 13:20:51 roberto Exp $
 ** Auxiliary functions for building Lua libraries
 ** See Copyright Notice in lua.h
 */
/**
 ** #define lauxlib_c
 ** #define LUA_LIB  
 */
namespace kurumi
{
	public class LoadF
	{
		public int extraline;
		public StreamProxy f;
		public LuaConf.CharPtr buff = LuaConf.CharPtr.toCharPtr(new char[LuaConf.LUAL_BUFFERSIZE]);
	}
}
