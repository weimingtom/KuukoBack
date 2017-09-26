/*
 ** $Id: luaconf.h,v 1.82.1.7 2008/02/11 16:25:08 roberto Exp $
 ** Configuration file for Lua
 ** See Copyright Notice in lua.h
 */
using System;

namespace kurumi
{
	public class LuaException : Exception
	{
		public LuaState.lua_State L;
		public LuaDo.lua_longjmp c;
	
		public LuaException(LuaState.lua_State L, LuaDo.lua_longjmp c) 
		{ 
			this.L = L; 
			this.c = c;
		}
	}
}
