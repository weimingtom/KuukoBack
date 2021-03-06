/*
 ** $Id: lua.h,v 1.218.1.5 2008/08/06 13:30:12 roberto Exp $
 ** Lua - An Extensible Extension Language
 ** Lua.org, PUC-Rio, Brazil (http://www.lua.org)
 ** See Copyright Notice at the end of this file
 */
using System;

namespace kurumi
{
	//using lua_Number = Double;
	//using lua_Integer = System.Int32;
	
	public class Lua
	{
		public const string LUA_VERSION = "Lua 5.1";
		public const string LUA_RELEASE = "Lua 5.1.4";
		public const int LUA_VERSION_NUM	= 501;
		public const string LUA_COPYRIGHT = "Copyright (C) 1994-2008 Lua.org, PUC-Rio";
		public const string LUA_AUTHORS = "R. Ierusalimschy, L. H. de Figueiredo & W. Celes";

		/* mark for precompiled code (`<esc>Lua') */
		public const string LUA_SIGNATURE = "\x01bLua";

		/* option for multiple returns in `lua_pcall' and `lua_call' */
		public const int LUA_MULTRET = (-1);

		/*
		 ** pseudo-indices
		 */
		public const int LUA_REGISTRYINDEX = (-10000);
		public const int LUA_ENVIRONINDEX = (-10001);
		public const int LUA_GLOBALSINDEX = (-10002);
		public static int lua_upvalueindex(int i)	
		{
			return LUA_GLOBALSINDEX - i;
		}
		
		/* thread status; 0 is OK */
		public const int LUA_YIELD = 1;
		public const int LUA_ERRRUN = 2;
		public const int LUA_ERRSYNTAX = 3;
		public const int LUA_ERRMEM	= 4;
		public const int LUA_ERRERR	= 5;

	    public interface lua_CFunction
	    {
	        int exec(LuaState.lua_State L);
	    }
		
	    public interface lua_Reader
	    {
	        /*sz*/
	        /*out*/
	        /*uint*/
	        CLib.CharPtr exec(LuaState.lua_State L, object ud, int[] sz);
	    }
		
	    // functions that read/write blocks when loading/dumping Lua chunks
		//public delegate int lua_Writer(lua_State L, CharPtr p, int//uint// sz, object ud);
		public interface lua_Writer
	    {
	        //uint sz
	        int exec(LuaState.lua_State L, CLib.CharPtr p, int sz, object ud);
	    }
		
	    public interface lua_Alloc
	    {
	        object exec(ClassType t);
	    }		
		
		/*
		 ** basic types
		 */
		public const int LUA_TNONE = -1;

		public const int LUA_TNIL = 0;
		public const int LUA_TBOOLEAN = 1;
		public const int LUA_TLIGHTUSERDATA = 2;
		public const int LUA_TNUMBER = 3;
		public const int LUA_TSTRING = 4;
		public const int LUA_TTABLE = 5;
		public const int LUA_TFUNCTION = 6;
		public const int LUA_TUSERDATA = 7;
		public const int LUA_TTHREAD = 8;

		/* minimum Lua stack available to a C function */
		public const int LUA_MINSTACK = 20;

		/* type of numbers in Lua */
		//typedef LUA_NUMBER lua_Number;

		/* type for integer functions */
		//typedef LUA_INTEGER lua_Integer;

		/*
		 ** garbage-collection function and options
		 */
		public const int LUA_GCSTOP			= 0;
		public const int LUA_GCRESTART		= 1;
		public const int LUA_GCCOLLECT		= 2;
		public const int LUA_GCCOUNT		= 3;
		public const int LUA_GCCOUNTB		= 4;
		public const int LUA_GCSTEP			= 5;
		public const int LUA_GCSETPAUSE		= 6;
		public const int LUA_GCSETSTEPMUL	= 7;

		/* 
		 ** ===============================================================
		 ** some useful macros
		 ** ===============================================================
		 */
		public static void lua_pop(LuaState.lua_State L, int n)
		{
			LuaAPI.lua_settop(L, -(n) - 1);
		}

		public static void lua_newtable(LuaState.lua_State L)
		{
			LuaAPI.lua_createtable(L, 0, 0);
		}

		public static void lua_register(LuaState.lua_State L, CLib.CharPtr n, lua_CFunction f)
		{
			lua_pushcfunction(L, f);
			lua_setglobal(L, n);
		}

		public static void lua_pushcfunction(LuaState.lua_State L, lua_CFunction f)
		{
			LuaAPI.lua_pushcclosure(L, f, 0);
		}

		public static int/*uint*/ lua_strlen(LuaState.lua_State L, int i)
		{
			return LuaAPI.lua_objlen(L, i);
		}

		public static bool lua_isfunction(LuaState.lua_State L, int n)
		{
			return LuaAPI.lua_type(L, n) == LUA_TFUNCTION;
		}

		public static bool lua_istable(LuaState.lua_State L, int n)
		{
			return LuaAPI.lua_type(L, n) == LUA_TTABLE;
		}

		public static bool lua_islightuserdata(LuaState.lua_State L, int n)
		{
			return LuaAPI.lua_type(L, n) == LUA_TLIGHTUSERDATA;
		}

		public static bool lua_isnil(LuaState.lua_State L, int n)
		{
			return LuaAPI.lua_type(L, n) == LUA_TNIL;
		}

		public static bool lua_isboolean(LuaState.lua_State L, int n)
		{
			return LuaAPI.lua_type(L, n) == LUA_TBOOLEAN;
		}

		public static bool lua_isthread(LuaState.lua_State L, int n)
		{
			return LuaAPI.lua_type(L, n) == LUA_TTHREAD;
		}

		public static bool lua_isnone(LuaState.lua_State L, int n)
		{
			return LuaAPI.lua_type(L, n) == LUA_TNONE;
		}

		public static bool lua_isnoneornil(LuaState.lua_State L, Double/*lua_Number*/ n)
		{
			return LuaAPI.lua_type(L, (int)n) <= 0;
		}

		public static void lua_pushliteral(LuaState.lua_State L, CLib.CharPtr s)
		{
			//TODO: Implement use using lua_pushlstring instead of lua_pushstring
			//lua_pushlstring(L, "" s, (sizeof(s)/GetUnmanagedSize(typeof(char)))-1)
			LuaAPI.lua_pushstring(L, s);
		}

		public static void lua_setglobal(LuaState.lua_State L, CLib.CharPtr s)
		{
			LuaAPI.lua_setfield(L, LUA_GLOBALSINDEX, s);
		}

		public static void lua_getglobal(LuaState.lua_State L, CLib.CharPtr s)
		{
			LuaAPI.lua_getfield(L, LUA_GLOBALSINDEX, s);
		}

		public static CLib.CharPtr lua_tostring(LuaState.lua_State L, int i)
		{
			int[]/*uint*/ blah = new int[1];
			return LuaAPI.lua_tolstring(L, i, /*out*/ blah);
		}

		////#define lua_open()	luaL_newstate()
		public static LuaState.lua_State lua_open()
		{
			return LuaAuxLib.luaL_newstate();
		}

		////#define lua_getregistry(L)	lua_pushvalue(L, LUA_REGISTRYINDEX)
		public static void lua_getregistry(LuaState.lua_State L)
		{
			LuaAPI.lua_pushvalue(L, LUA_REGISTRYINDEX);
		}

		////#define lua_getgccount(L)	lua_gc(L, LUA_GCCOUNT, 0)
		public static int lua_getgccount(LuaState.lua_State L)
		{
			return LuaAPI.lua_gc(L, LUA_GCCOUNT, 0);
		}

		//#define lua_Chunkreader		lua_Reader
		//#define lua_Chunkwriter		lua_Writer


		/*
		 ** {======================================================================
		 ** Debug API
		 ** =======================================================================
		 */


		/*
		 ** Event codes
		 */
		public const int LUA_HOOKCALL = 0;
		public const int LUA_HOOKRET = 1;
		public const int LUA_HOOKLINE = 2;
		public const int LUA_HOOKCOUNT = 3;
		public const int LUA_HOOKTAILRET = 4;


		/*
		 ** Event masks
		 */
		public const int LUA_MASKCALL = (1 << LUA_HOOKCALL);
		public const int LUA_MASKRET = (1 << LUA_HOOKRET);
		public const int LUA_MASKLINE = (1 << LUA_HOOKLINE);
		public const int LUA_MASKCOUNT = (1 << LUA_HOOKCOUNT);

	    /* Functions to be called by the debuger in specific events */
	    //public delegate void lua_Hook(lua_State L, lua_Debug ar);
	    public interface lua_Hook
	    {
	        void exec(LuaState.lua_State L, Lua.lua_Debug ar);
	    }
				
		public class lua_Debug
		{
			public int event_;
			public CLib.CharPtr name;	/* (n) */
			public CLib.CharPtr namewhat;	/* (n) `global', `local', `field', `method' */
			public CLib.CharPtr what;	/* (S) `Lua', `C', `main', `tail' */
			public CLib.CharPtr source;	/* (S) */
			public int currentline;	/* (l) */
			public int nups;		/* (u) number of upvalues */
			public int linedefined;	/* (S) */
			public int lastlinedefined;	/* (S) */
			public CLib.CharPtr short_src = CLib.CharPtr.toCharPtr(new char[LuaConf.LUA_IDSIZE]); /* (S) */
			/* private part */
			public int i_ci;  /* active function */
		}		
		
		
		/* }====================================================================== */

		/******************************************************************************
		 * Copyright (C) 1994-2008 Lua.org, PUC-Rio.  All rights reserved.
		 *
		 * Permission is hereby granted, free of charge, to any person obtaining
		 * a copy of this software and associated documentation files (the
		 * "Software"), to deal in the Software without restriction, including
		 * without limitation the rights to use, copy, modify, merge, publish,
		 * distribute, sublicense, and/or sell copies of the Software, and to
		 * permit persons to whom the Software is furnished to do so, subject to
		 * the following conditions:
		 *
		 * The above copyright notice and this permission notice shall be
		 * included in all copies or substantial portions of the Software.
		 *
		 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
		 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
		 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
		 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
		 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
		 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
		 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
		 ******************************************************************************/
	}

	//public delegate int lua_CFunction(lua_State L);
	//public delegate CharPtr lua_Reader(lua_State L, object ud, out int/*uint*/ sz);

	//public interface lua_CFunction
	//{
	//	int exec(lua_State L);
    //}
	//public interface lua_Reader
	//{
	//	CharPtr exec(lua_State L, object ud, /*out*/ int[]/*uint*/ sz);
	//}
	
	/*
	 ** prototype for memory-allocation functions
	 */
	//public delegate object lua_Alloc(object ud, object ptr, uint osize, uint nsize);
	
	//public delegate object lua_Alloc(Type t);
	//public interface lua_Alloc
	//{
	//	object exec(ClassType t);
	//}
	
	
	/*
	 ** functions that read/write blocks when loading/dumping Lua chunks
	 */
	//public delegate int lua_Writer(lua_State L, CharPtr p, int/*uint*/ sz, object ud);
	//public interface lua_Writer
	//{
	//	int exec(lua_State L, CharPtr p, int/*uint*/ sz, object ud);
    //}
	
	
	/* Functions to be called by the debuger in specific events */
	//public delegate void lua_Hook(lua_State L, lua_Debug ar);
	//public interface lua_Hook
	//{
	//	void exec(lua_State L, lua_Debug ar);
    //}
}
