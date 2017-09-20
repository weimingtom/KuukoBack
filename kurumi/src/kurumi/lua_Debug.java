/*
 ** $Id: lua.h,v 1.218.1.5 2008/08/06 13:30:12 roberto Exp $
 ** Lua - An Extensible Extension Language
 ** Lua.org, PUC-Rio, Brazil (http://www.lua.org)
 ** See Copyright Notice at the end of this file
 */
package kurumi;
//{
	public class lua_Debug
	{
		public int event_;
		public LuaConf.CharPtr name;	/* (n) */
		public LuaConf.CharPtr namewhat;	/* (n) `global', `local', `field', `method' */
		public LuaConf.CharPtr what;	/* (S) `Lua', `C', `main', `tail' */
		public LuaConf.CharPtr source;	/* (S) */
		public int currentline;	/* (l) */
		public int nups;		/* (u) number of upvalues */
		public int linedefined;	/* (S) */
		public int lastlinedefined;	/* (S) */
		public LuaConf.CharPtr short_src = LuaConf.CharPtr.toCharPtr(new char[LuaConf.LUA_IDSIZE]); /* (S) */
		/* private part */
		public int i_ci;  /* active function */
	}
//}
