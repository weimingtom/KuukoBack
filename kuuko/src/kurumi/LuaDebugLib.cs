/*
 ** $Id: ldblib.c,v 1.104.1.3 2008/01/21 13:11:21 roberto Exp $
 ** Interface from Lua to its debug API
 ** See Copyright Notice in lua.h
 */
namespace kurumi
{
	public class LuaDebugLib
	{
		private static int db_getregistry(LuaState.lua_State L) 
		{
			LuaAPI.lua_pushvalue(L, Lua.LUA_REGISTRYINDEX);
			return 1;
		}

		private static int db_getmetatable(LuaState.lua_State L) 
		{
			LuaAuxLib.luaL_checkany(L, 1);
			if (LuaAPI.lua_getmetatable(L, 1) == 0)
			{
				LuaAPI.lua_pushnil(L);  /* no metatable */
			}
			return 1;
		}

		private static int db_setmetatable(LuaState.lua_State L) 
		{
			int t = LuaAPI.lua_type(L, 2);
			LuaAuxLib.luaL_argcheck(L, t == Lua.LUA_TNIL || t == Lua.LUA_TTABLE, 2,
				"nil or table expected");
			LuaAPI.lua_settop(L, 2);
			LuaAPI.lua_pushboolean(L, LuaAPI.lua_setmetatable(L, 1));
			return 1;
		}

		private static int db_getfenv(LuaState.lua_State L) 
		{
			LuaAPI.lua_getfenv(L, 1);
			return 1;
		}


		private static int db_setfenv(LuaState.lua_State L) 
		{
			LuaAuxLib.luaL_checktype(L, 2, Lua.LUA_TTABLE);
			LuaAPI.lua_settop(L, 2);
			if (LuaAPI.lua_setfenv(L, 1) == 0)
			{	
				LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr(LuaConf.LUA_QL("setfenv") +
					" cannot change environment of given object"));
			}
			return 1;
		}

		private static void settabss(LuaState.lua_State L, CLib.CharPtr i, CLib.CharPtr v) 
		{
			LuaAPI.lua_pushstring(L, v);
			LuaAPI.lua_setfield(L, -2, i);
		}

		private static void settabsi(LuaState.lua_State L, CLib.CharPtr i, int v) 
		{
			LuaAPI.lua_pushinteger(L, v);
			LuaAPI.lua_setfield(L, -2, i);
		}

		private static LuaState.lua_State getthread(LuaState.lua_State L, /*out*/ int[] arg)
		{
			if (Lua.lua_isthread(L, 1)) 
			{
				arg[0] = 1;
				return LuaAPI.lua_tothread(L, 1);
			}
			else 
			{
				arg[0] = 0;
				return L;
			}
		}

		private static void treatstackoption(LuaState.lua_State L, LuaState.lua_State L1, CLib.CharPtr fname) 
		{
			if (L == L1) 
			{
				LuaAPI.lua_pushvalue(L, -2);
				LuaAPI.lua_remove(L, -3);
			}
			else
			{
				LuaAPI.lua_xmove(L1, L, 1);
			}
			LuaAPI.lua_setfield(L, -2, fname);
		}

		private static int db_getinfo(LuaState.lua_State L) 
		{
			Lua.lua_Debug ar = new Lua.lua_Debug();
			int[] arg = new int[1];
			LuaState.lua_State L1 = getthread(L, /*out*/ arg);
			CLib.CharPtr options = LuaAuxLib.luaL_optstring(L, arg[0] + 2, CLib.CharPtr.toCharPtr("flnSu"));
			if (LuaAPI.lua_isnumber(L, arg[0] + 1) != 0)
			{
				if (LuaDebug.lua_getstack(L1, (int)LuaAPI.lua_tointeger(L, arg[0] + 1), ar) == 0)
				{
					LuaAPI.lua_pushnil(L);  /* level out of range */
					return 1;
				}
			}
			else if (Lua.lua_isfunction(L, arg[0] + 1))
			{
				LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr(">%s"), options);
				options = Lua.lua_tostring(L, -1);
				LuaAPI.lua_pushvalue(L, arg[0] + 1);
				LuaAPI.lua_xmove(L, L1, 1);
			}
			else
			{
				return LuaAuxLib.luaL_argerror(L, arg[0] + 1, CLib.CharPtr.toCharPtr("function or level expected"));
			}
			if (LuaDebug.lua_getinfo(L1, options, ar) == 0)
			{
				return LuaAuxLib.luaL_argerror(L, arg[0] + 2, CLib.CharPtr.toCharPtr("invalid option"));
			}
			LuaAPI.lua_createtable(L, 0, 2);
			if (CLib.CharPtr.isNotEqual(CLib.strchr(options, 'S'), null))
			{
				settabss(L, CLib.CharPtr.toCharPtr("source"), ar.source);
				settabss(L, CLib.CharPtr.toCharPtr("short_src"), ar.short_src);
				settabsi(L, CLib.CharPtr.toCharPtr("linedefined"), ar.linedefined);
				settabsi(L, CLib.CharPtr.toCharPtr("lastlinedefined"), ar.lastlinedefined);
				settabss(L, CLib.CharPtr.toCharPtr("what"), ar.what);
			}
			if (CLib.CharPtr.isNotEqual(CLib.strchr(options, 'l'), null))
			{
				settabsi(L, CLib.CharPtr.toCharPtr("currentline"), ar.currentline);
			}
			if (CLib.CharPtr.isNotEqual(CLib.strchr(options, 'u'), null))
			{
				settabsi(L, CLib.CharPtr.toCharPtr("nups"), ar.nups);
			}
			if (CLib.CharPtr.isNotEqual(CLib.strchr(options, 'n'), null))
			{
				settabss(L, CLib.CharPtr.toCharPtr("name"), ar.name);
				settabss(L, CLib.CharPtr.toCharPtr("namewhat"), ar.namewhat);
			}
			if (CLib.CharPtr.isNotEqual(CLib.strchr(options, 'L'), null))
			{
				treatstackoption(L, L1, CLib.CharPtr.toCharPtr("activelines"));
			}
			if (CLib.CharPtr.isNotEqual(CLib.strchr(options, 'f'), null))
			{
				treatstackoption(L, L1, CLib.CharPtr.toCharPtr("func"));
			}
			return 1;  /* return table */
		}
		
		private static int db_getlocal(LuaState.lua_State L) 
		{
			int[] arg = new int[1];
			LuaState.lua_State L1 = getthread(L, /*out*/ arg);
			Lua.lua_Debug ar = new Lua.lua_Debug();
			CLib.CharPtr name;
			if (LuaDebug.lua_getstack(L1, LuaAuxLib.luaL_checkint(L, arg[0] + 1), ar) == 0)  /* out of range? */
			{
				return LuaAuxLib.luaL_argerror(L, arg[0] + 1, CLib.CharPtr.toCharPtr("level out of range"));
			}
			name = LuaDebug.lua_getlocal(L1, ar, LuaAuxLib.luaL_checkint(L, arg[0] + 2));
			if (CLib.CharPtr.isNotEqual(name, null)) 
			{
				LuaAPI.lua_xmove(L1, L, 1);
				LuaAPI.lua_pushstring(L, name);
				LuaAPI.lua_pushvalue(L, -2);
				return 2;
			}
			else 
			{
				LuaAPI.lua_pushnil(L);
				return 1;
			}
		}

		private static int db_setlocal(LuaState.lua_State L) 
		{
			int[] arg = new int[1];
			LuaState.lua_State L1 = getthread(L, /*out*/ arg);
			Lua.lua_Debug ar = new Lua.lua_Debug();
			if (LuaDebug.lua_getstack(L1, LuaAuxLib.luaL_checkint(L, arg[0] + 1), ar) == 0)  /* out of range? */
			{
				return LuaAuxLib.luaL_argerror(L, arg[0] + 1, CLib.CharPtr.toCharPtr("level out of range"));
			}
			LuaAuxLib.luaL_checkany(L, arg[0] + 3);
			LuaAPI.lua_settop(L, arg[0] + 3);
			LuaAPI.lua_xmove(L, L1, 1);
			LuaAPI.lua_pushstring(L, LuaDebug.lua_setlocal(L1, ar, LuaAuxLib.luaL_checkint(L, arg[0] + 2)));
			return 1;
		}

		private static int auxupvalue(LuaState.lua_State L, int get) 
		{
			CLib.CharPtr name;
			int n = LuaAuxLib.luaL_checkint(L, 2);
			LuaAuxLib.luaL_checktype(L, 1, Lua.LUA_TFUNCTION);
			if (LuaAPI.lua_iscfunction(L, 1)) return 0;  /* cannot touch C upvalues from Lua */
			{
				name = (get != 0) ? LuaAPI.lua_getupvalue(L, 1, n) : LuaAPI.lua_setupvalue(L, 1, n);
			}
			if (CLib.CharPtr.isEqual(name, null)) 
			{
				return 0;
			}
			LuaAPI.lua_pushstring(L, name);
			LuaAPI.lua_insert(L, -(get + 1));
			return get + 1;
		}

		private static int db_getupvalue(LuaState.lua_State L) 
		{
			return auxupvalue(L, 1);
		}

		private static int db_setupvalue(LuaState.lua_State L) 
		{
			LuaAuxLib.luaL_checkany(L, 3);
			return auxupvalue(L, 0);
		}
		
		private const string KEY_HOOK = "h";

		private static readonly string[] hooknames = {
			"call", 
			"return", 
			"line", 
			"count", 
			"tail return"
		};

		private static void hookf(LuaState.lua_State L, Lua.lua_Debug ar) 
		{
			LuaAPI.lua_pushlightuserdata(L, KEY_HOOK);
			LuaAPI.lua_rawget(L, Lua.LUA_REGISTRYINDEX);
			LuaAPI.lua_pushlightuserdata(L, L);
			LuaAPI.lua_rawget(L, -2);
			if (Lua.lua_isfunction(L, -1))
			{
				LuaAPI.lua_pushstring(L, CLib.CharPtr.toCharPtr(hooknames[(int)ar.event_]));
				if (ar.currentline >= 0)
				{
					LuaAPI.lua_pushinteger(L, ar.currentline);
				}
				else
				{
					LuaAPI.lua_pushnil(L);
				}
				LuaLimits.lua_assert(LuaDebug.lua_getinfo(L, CLib.CharPtr.toCharPtr("lS"), ar));
				LuaAPI.lua_call(L, 2, 0);
			}
		}

		public class hookf_delegate : Lua.lua_Hook
		{
			public void exec(LuaState.lua_State L, Lua.lua_Debug ar)
			{
				hookf(L, ar);
			}
		}
		
		private static int makemask(CLib.CharPtr smask, int count) 
		{
			int mask = 0;
			if (CLib.CharPtr.isNotEqual(CLib.strchr(smask, 'c'), null)) 
			{
				mask |= Lua.LUA_MASKCALL;
			}
			if (CLib.CharPtr.isNotEqual(CLib.strchr(smask, 'r'), null)) 
			{
				mask |= Lua.LUA_MASKRET;
			}
			if (CLib.CharPtr.isNotEqual(CLib.strchr(smask, 'l'), null)) 
			{
				mask |= Lua.LUA_MASKLINE;
			}
			if (count > 0) 
			{
				mask |= Lua.LUA_MASKCOUNT;
			}
			return mask;
		}

		private static CLib.CharPtr unmakemask (int mask, CLib.CharPtr smask) 
		{
			int i = 0;
			if ((mask & Lua.LUA_MASKCALL) != 0) 
			{
				smask.set(i++, 'c');
			}
			if ((mask & Lua.LUA_MASKRET) != 0) 
			{
				smask.set(i++, 'r');
			}
			if ((mask & Lua.LUA_MASKLINE) != 0) 
			{
				smask.set(i++, 'l');
			}
			smask.set(i, '\0');
			return smask;
		}

		private static void gethooktable (LuaState.lua_State L) 
		{
			LuaAPI.lua_pushlightuserdata(L, KEY_HOOK);
			LuaAPI.lua_rawget(L, Lua.LUA_REGISTRYINDEX);
			if (!Lua.lua_istable(L, -1))
			{
				Lua.lua_pop(L, 1);
				LuaAPI.lua_createtable(L, 0, 1);
				LuaAPI.lua_pushlightuserdata(L, KEY_HOOK);
				LuaAPI.lua_pushvalue(L, -2);
				LuaAPI.lua_rawset(L, Lua.LUA_REGISTRYINDEX);
			}
		}

		private static int db_sethook(LuaState.lua_State L) 
		{
			int[] arg = new int[1];
			int mask, count;
			Lua.lua_Hook func;
			LuaState.lua_State L1 = getthread(L, /*out*/ arg);
			if (Lua.lua_isnoneornil(L, arg[0] + 1)) 
			{
				LuaAPI.lua_settop(L, arg[0] + 1);
				func = null; 
				mask = 0; 
				count = 0;  /* turn off hooks */
			}
			else 
			{
				CLib.CharPtr smask = LuaAuxLib.luaL_checkstring(L, arg[0] + 2);
				LuaAuxLib.luaL_checktype(L, arg[0] + 1, Lua.LUA_TFUNCTION);
				count = LuaAuxLib.luaL_optint(L, arg[0] + 3, 0);
				func = new hookf_delegate();
				mask = makemask(smask, count);
			}
			gethooktable(L);
			LuaAPI.lua_pushlightuserdata(L, L1);
			LuaAPI.lua_pushvalue(L, arg[0] + 1);
			LuaAPI.lua_rawset(L, -3);  /* set new hook */
			Lua.lua_pop(L, 1);  /* remove hook table */
			LuaDebug.lua_sethook(L1, func, mask, count);  /* set hooks */
			return 0;
		}

		private static int db_gethook(LuaState.lua_State L) 
		{
			int[] arg = new int[1];
			LuaState.lua_State L1 = getthread(L, /*out*/ arg);
			CLib.CharPtr buff = CLib.CharPtr.toCharPtr(new char[5]);
			int mask = LuaDebug.lua_gethookmask(L1);
			Lua.lua_Hook hook = LuaDebug.lua_gethook(L1);
			if (hook != null && (hook is hookf_delegate))  /* external hook? */
			{
				Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr("external hook"));
			}
			else 
			{
				gethooktable(L);
				LuaAPI.lua_pushlightuserdata(L, L1);
				LuaAPI.lua_rawget(L, -2);   /* get hook */
				LuaAPI.lua_remove(L, -2);  /* remove hook table */
			}
			LuaAPI.lua_pushstring(L, unmakemask(mask, buff));
			LuaAPI.lua_pushinteger(L, LuaDebug.lua_gethookcount(L1));
			return 3;
		}

		private static int db_debug(LuaState.lua_State L) 
		{
			for (;;) 
			{
				CLib.CharPtr buffer = CLib.CharPtr.toCharPtr(new char[250]);
				CLib.fputs(CLib.CharPtr.toCharPtr("lua_debug> "), CLib.stderr);
				if (CLib.CharPtr.isEqual(CLib.fgets(buffer, CLib.stdin), null) ||
				    CLib.strcmp(buffer, CLib.CharPtr.toCharPtr("cont\n")) == 0)
				{
					return 0;
				}
				if (LuaAuxLib.luaL_loadbuffer(L, buffer, /*(uint)*/CLib.strlen(buffer), CLib.CharPtr.toCharPtr("=(debug command)")) != 0 ||
				    LuaAPI.lua_pcall(L, 0, 0, 0) != 0)
				{
					CLib.fputs(Lua.lua_tostring(L, -1), CLib.stderr);
					CLib.fputs(CLib.CharPtr.toCharPtr("\n"), CLib.stderr);
				}
				LuaAPI.lua_settop(L, 0);  /* remove eventual returns */
			}
		}

		public const int LEVELS1 = 12;	/* size of the first part of the stack */
		public const int LEVELS2 = 10;	/* size of the second part of the stack */

		private static int db_errorfb(LuaState.lua_State L) 
		{
			int level;
			bool firstpart = true;  /* still before eventual `...' */
			int[] arg = new int[1];
			LuaState.lua_State L1 = getthread(L, /*out*/ arg);
			Lua.lua_Debug ar = new Lua.lua_Debug();
			if (LuaAPI.lua_isnumber(L, arg[0] + 2) != 0)
			{
				level = (int)LuaAPI.lua_tointeger(L, arg[0] + 2);
				Lua.lua_pop(L, 1);
			}
			else
			{
				level = (L == L1) ? 1 : 0;  /* level 0 may be this own function */
			}
			if (LuaAPI.lua_gettop(L) == arg[0])
			{
				Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr(""));
			}
			else if (LuaAPI.lua_isstring(L, arg[0] + 1) == 0) 
			{
				return 1;  /* message is not a string */
			}
			else 
			{
				Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr("\n"));
			}
			Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr("stack traceback:"));
			while (LuaDebug.lua_getstack(L1, level++, ar) != 0)
			{
				if (level > LEVELS1 && firstpart) 
				{
					/* no more than `LEVELS2' more levels? */
					if (LuaDebug.lua_getstack(L1, level + LEVELS2, ar) == 0)
					{
						level--;  /* keep going */
					}
					else 
					{
						Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr("\n\t..."));  /* too many levels */
						while (LuaDebug.lua_getstack(L1, level + LEVELS2, ar) != 0)  /* find last levels */
						{
							level++;
						}
					}
					firstpart = false;
					continue;
				}
				Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr("\n\t"));
				LuaDebug.lua_getinfo(L1, CLib.CharPtr.toCharPtr("Snl"), ar);
				LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr("%s:"), ar.short_src);
				if (ar.currentline > 0)
				{
					LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr("%d:"), ar.currentline);
				}
				if (CLib.CharPtr.isNotEqualChar(ar.namewhat, '\0'))  /* is there a name? */
				{
					LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr(" in function " + LuaConf.getLUA_QS()), ar.name);
				}
				else 
				{
					if (CLib.CharPtr.isEqualChar(ar.what, 'm'))  /* main? */
					{
						LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr(" in main chunk"));
					}
					else if (CLib.CharPtr.isEqualChar(ar.what, 'C') || CLib.CharPtr.isEqualChar(ar.what, 't'))
					{
						Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr(" ?"));  /* C function or tail call */
					}
					else
					{
						LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr(" in function <%s:%d>"),
							ar.short_src, ar.linedefined);
					}
				}
				LuaAPI.lua_concat(L, LuaAPI.lua_gettop(L) - arg[0]);
			}
			LuaAPI.lua_concat(L, LuaAPI.lua_gettop(L) - arg[0]);
			return 1;
		}

		private readonly static LuaAuxLib.luaL_Reg[] dblib = {
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("debug"), new LuaDebugLib_delegate("db_debug")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("getfenv"), new LuaDebugLib_delegate("db_getfenv")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("gethook"), new LuaDebugLib_delegate("db_gethook")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("getinfo"), new LuaDebugLib_delegate("db_getinfo")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("getlocal"), new LuaDebugLib_delegate("db_getlocal")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("getregistry"), new LuaDebugLib_delegate("db_getregistry")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("getmetatable"), new LuaDebugLib_delegate("db_getmetatable")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("getupvalue"), new LuaDebugLib_delegate("db_getupvalue")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("setfenv"), new LuaDebugLib_delegate("db_setfenv")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("sethook"), new LuaDebugLib_delegate("db_sethook")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("setlocal"), new LuaDebugLib_delegate("db_setlocal")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("setmetatable"), new LuaDebugLib_delegate("db_setmetatable")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("setupvalue"), new LuaDebugLib_delegate("db_setupvalue")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("traceback"), new LuaDebugLib_delegate("db_errorfb")),
			new LuaAuxLib.luaL_Reg(null, null)
		};
		
		public class LuaDebugLib_delegate : Lua.lua_CFunction
		{
			private string name;
			
			public LuaDebugLib_delegate(string name)
			{
				this.name = name;
			}
			
			public int exec(LuaState.lua_State L)
			{
				if ("db_debug".Equals(name))
				{
					return db_debug(L);
				} 
				else if ("db_getfenv".Equals(name)) 
				{
					return db_getfenv(L);
				} 
				else if ("db_gethook".Equals(name)) 
				{
					return db_gethook(L);
				} 
				else if ("db_getinfo".Equals(name)) 
				{
				    return db_getinfo(L);
				}
				else if ("db_getlocal".Equals(name))
				{
				    return db_getlocal(L);
				}
				else if ("db_getregistry".Equals(name))
				{
					return db_getregistry(L);
				}
				else if ("db_getmetatable".Equals(name))
				{
					return db_getmetatable(L);
				}
				else if ("db_getupvalue".Equals(name))
				{
					return db_getupvalue(L);
				}
				else if ("db_setfenv".Equals(name))
				{
					return db_setfenv(L);
				}
				else if ("db_sethook".Equals(name))
				{
					return db_sethook(L);
				}
				else if ("db_setlocal".Equals(name))
				{
					return db_setlocal(L);
				}
				else if ("db_setmetatable".Equals(name))
				{
					return db_setmetatable(L);
				}
				else if ("db_setupvalue".Equals(name))
				{
					return db_setupvalue(L);
				}
				else if ("db_errorfb".Equals(name))
				{
					return db_errorfb(L);
				}
				else
				{
					return 0;
				}
			}
		}


		public static int luaopen_debug (LuaState.lua_State L) {
			LuaAuxLib.luaL_register(L, CLib.CharPtr.toCharPtr(LuaLib.LUA_DBLIBNAME), dblib);
			return 1;
		}

	}
}
