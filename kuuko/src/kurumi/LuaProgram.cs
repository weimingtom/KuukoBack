/*
 ** $Id: lua.c,v 1.160.1.2 2007/12/28 15:32:23 roberto Exp $
 ** Lua stand-alone interpreter
 ** See Copyright Notice in lua.h
 */
using System;

namespace kurumi
{
	public class LuaProgram
	{
		//#define lua_c

		//#include "lua.h"

		//#include "lauxlib.h"
		//#include "lualib.h"

		static LuaState.lua_State globalL = null;

		static LuaConf.CharPtr progname = LuaConf.CharPtr.toCharPtr(LuaConf.LUA_PROGNAME);

		static void lstop(LuaState.lua_State L, Lua.lua_Debug ar)
		{
			LuaDebug.lua_sethook(L, null, 0, 0);
			LuaAuxLib.luaL_error(L, LuaConf.CharPtr.toCharPtr("interrupted!"));
		}
		
		public class lstop_delegate : Lua.lua_Hook
		{
			public void exec(LuaState.lua_State L, Lua.lua_Debug ar)
			{
				lstop(L, ar);
			}
		}


		static void laction(int i)
		{
			//signal(i, SIG_DFL); /* if another SIGINT happens before lstop,
			//						  terminate process (default action) */
			LuaDebug.lua_sethook(globalL, new lstop_delegate(), Lua.LUA_MASKCALL | Lua.LUA_MASKRET | Lua.LUA_MASKCOUNT, 1);
		}

		static void print_usage()
		{
			StreamProxy.ErrorWrite(
				"usage: " + progname.ToString() + " [options] [script [args]].\n" +
				"Available options are:\n" +
				"  -e stat  execute string " + LuaConf.LUA_QL("stat").ToString() + "\n" +
				"  -l name  require library " + LuaConf.LUA_QL("name").ToString() + "\n" +
				"  -i       enter interactive mode after executing " + LuaConf.LUA_QL("script").ToString() + "\n" +
				"  -v       show version information\n" +
				"  --       stop handling options\n" +
				"  -        execute stdin and stop handling options\n");
		}

		static void l_message(LuaConf.CharPtr pname, LuaConf.CharPtr msg)
		{
			if (LuaConf.CharPtr.isNotEqual(pname, null)) 
			{
				LuaConf.fprintf(LuaConf.stderr, LuaConf.CharPtr.toCharPtr("%s: "), pname);
			}
			LuaConf.fprintf(LuaConf.stderr, LuaConf.CharPtr.toCharPtr("%s\n"), msg);
			LuaConf.fflush(LuaConf.stderr);
		}

		static int report(LuaState.lua_State L, int status)
		{
			if ((status != 0) && !Lua.lua_isnil(L, -1))
			{
				LuaConf.CharPtr msg = Lua.lua_tostring(L, -1);
				if (LuaConf.CharPtr.isEqual(msg, null)) 
				{
					msg = LuaConf.CharPtr.toCharPtr("(error object is not a string)");
				}
				l_message(progname, msg);
				Lua.lua_pop(L, 1);
			}
			return status;
		}

		static int traceback(LuaState.lua_State L)
		{
			if (LuaAPI.lua_isstring(L, 1) == 0)  /* 'message' not a string? */
			{
				return 1;  /* keep it intact */
			}
			LuaAPI.lua_getfield(L, Lua.LUA_GLOBALSINDEX, LuaConf.CharPtr.toCharPtr("debug"));
			if (!Lua.lua_istable(L, -1))
			{
				Lua.lua_pop(L, 1);
				return 1;
			}
			LuaAPI.lua_getfield(L, -1, LuaConf.CharPtr.toCharPtr("traceback"));
			if (!Lua.lua_isfunction(L, -1))
			{
				Lua.lua_pop(L, 2);
				return 1;
			}
			LuaAPI.lua_pushvalue(L, 1);  /* pass error message */
			LuaAPI.lua_pushinteger(L, 2);  /* skip this function and traceback */
			LuaAPI.lua_call(L, 2, 1);  /* call debug.traceback */
			return 1;
		}

		static int docall(LuaState.lua_State L, int narg, int clear)
		{
			int status;
			int base_ = LuaAPI.lua_gettop(L) - narg;  /* function index */
			Lua.lua_pushcfunction(L, new traceback_delegate());  /* push traceback function */
			LuaAPI.lua_insert(L, base_);  /* put it under chunk and args */
			//signal(SIGINT, laction);
			status = LuaAPI.lua_pcall(L, narg, ((clear != 0) ? 0 : Lua.LUA_MULTRET), base_);
			//signal(SIGINT, SIG_DFL);
			LuaAPI.lua_remove(L, base_);  /* remove traceback function */
			/* force a complete garbage collection in case of errors */
			if (status != 0) 
			{
				LuaAPI.lua_gc(L, Lua.LUA_GCCOLLECT, 0);
			}
			return status;
		}

		static void print_version()
		{
			l_message(null, LuaConf.CharPtr.toCharPtr(Lua.LUA_RELEASE + "  " + Lua.LUA_COPYRIGHT));
		}

		static int getargs(LuaState.lua_State L, string[] argv, int n)
		{
			int narg;
			int i;
			int argc = argv.Length;	/* count total number of arguments */
			narg = argc - (n + 1);  /* number of arguments to the script */
			LuaAuxLib.luaL_checkstack(L, narg + 3, LuaConf.CharPtr.toCharPtr("too many arguments to script"));
			for (i = n + 1; i < argc; i++)
			{
				LuaAPI.lua_pushstring(L, LuaConf.CharPtr.toCharPtr(argv[i]));
			}
			LuaAPI.lua_createtable(L, narg, n + 1);
			for (i = 0; i < argc; i++)
			{
				LuaAPI.lua_pushstring(L, LuaConf.CharPtr.toCharPtr(argv[i]));
				LuaAPI.lua_rawseti(L, -2, i - n);
			}
			return narg;
		}

		static int dofile(LuaState.lua_State L, LuaConf.CharPtr name)
		{
			int status = (LuaAuxLib.luaL_loadfile(L, name) != 0) || (docall(L, 0, 1) != 0) ? 1 : 0;
			return report(L, status);
		}

		static int dostring(LuaState.lua_State L, LuaConf.CharPtr s, LuaConf.CharPtr name)
		{
			int status = (LuaAuxLib.luaL_loadbuffer(L, s, /*(uint)*/LuaConf.strlen(s), name) != 0) || (docall(L, 0, 1) != 0) ? 1 : 0;
			return report(L, status);
		}

		static int dolibrary(LuaState.lua_State L, LuaConf.CharPtr name)
		{
			Lua.lua_getglobal(L, LuaConf.CharPtr.toCharPtr("require"));
			LuaAPI.lua_pushstring(L, name);
			return report(L, docall(L, 1, 1));
		}

		static LuaConf.CharPtr get_prompt(LuaState.lua_State L, int firstline)
		{
			LuaConf.CharPtr p;
			LuaAPI.lua_getfield(L, Lua.LUA_GLOBALSINDEX, (firstline != 0) ? LuaConf.CharPtr.toCharPtr("_PROMPT") : LuaConf.CharPtr.toCharPtr("_PROMPT2"));
			p = Lua.lua_tostring(L, -1);
			if (LuaConf.CharPtr.isEqual(p, null)) 
			{
				p = ((firstline != 0) ? LuaConf.CharPtr.toCharPtr(LuaConf.LUA_PROMPT) : LuaConf.CharPtr.toCharPtr(LuaConf.LUA_PROMPT2));
			}
			Lua.lua_pop(L, 1);  /* remove global */
			return p;
		}

		static int incomplete(LuaState.lua_State L, int status)
		{
			if (status == Lua.LUA_ERRSYNTAX)
			{
				int[]/*uint*/ lmsg = new int[1];
				LuaConf.CharPtr msg = LuaAPI.lua_tolstring(L, -1, /*out*/ lmsg);
				LuaConf.CharPtr tp = LuaConf.CharPtr.plus(msg, lmsg[0] - (LuaConf.strlen(LuaConf.LUA_QL("<eof>"))));
				if (LuaConf.CharPtr.isEqual(LuaConf.strstr(msg, LuaConf.LUA_QL("<eof>")), tp))
				{
					Lua.lua_pop(L, 1);
					return 1;
				}
			}
			return 0;  /* else... */
		}

		static int pushline(LuaState.lua_State L, int firstline)
		{
			LuaConf.CharPtr buffer = LuaConf.CharPtr.toCharPtr(new char[LuaConf.LUA_MAXINPUT]);
			LuaConf.CharPtr b = new LuaConf.CharPtr(buffer);
			int l;
			LuaConf.CharPtr prmt = get_prompt(L, firstline);
			if (!LuaConf.lua_readline(L, b, prmt))
			{
				return 0;  /* no input */
			}
			l = LuaConf.strlen(b);
			if (l > 0 && b.get(l - 1) == '\n')  /* line ends with newline? */
			{
				b.set(l - 1, '\0');  /* remove it */
			}
			if ((firstline != 0) && (b.get(0) == '='))  /* first line starts with `=' ? */
			{
				LuaAPI.lua_pushfstring(L, LuaConf.CharPtr.toCharPtr("return %s"), LuaConf.CharPtr.plus(b, 1));  /* change it to `return' */
			}
			else
			{
				LuaAPI.lua_pushstring(L, b);
			}
			LuaConf.lua_freeline(L, b);
			return 1;
		}

		static int loadline(LuaState.lua_State L)
		{
			int status;
			LuaAPI.lua_settop(L, 0);
			if (pushline(L, 1) == 0)
			{
				return -1;  /* no input */
			}
			for (;;)
			{  
				/* repeat until gets a complete line */
				status = LuaAuxLib.luaL_loadbuffer(L, Lua.lua_tostring(L, 1), Lua.lua_strlen(L, 1), LuaConf.CharPtr.toCharPtr("=stdin"));
				if (incomplete(L, status) == 0) 
				{
					break;  /* cannot try to add lines? */
				}
				if (pushline(L, 0)==0)  /* no more input? */
				{
					return -1;
				}
				Lua.lua_pushliteral(L, LuaConf.CharPtr.toCharPtr("\n"));  /* add a new line... */
				LuaAPI.lua_insert(L, -2);  /* ...between the two lines */
				LuaAPI.lua_concat(L, 3);  /* join them */
			}
			LuaConf.lua_saveline(L, 1);
			LuaAPI.lua_remove(L, 1);  /* remove line */
			return status;
		}

		static void dotty(LuaState.lua_State L)
		{
			int status;
			LuaConf.CharPtr oldprogname = progname;
			progname = null;
			while ((status = loadline(L)) != -1)
			{
				if (status == 0) 
				{
					status = docall(L, 0, 0);
				}
				report(L, status);
				if (status == 0 && LuaAPI.lua_gettop(L) > 0)
				{  
					/* any result to print? */
					Lua.lua_getglobal(L, LuaConf.CharPtr.toCharPtr("print"));
					LuaAPI.lua_insert(L, 1);
					if (LuaAPI.lua_pcall(L, LuaAPI.lua_gettop(L) - 1, 0, 0) != 0)
					{
						l_message(progname, LuaAPI.lua_pushfstring(L,
							LuaConf.CharPtr.toCharPtr("error calling " + LuaConf.LUA_QL("print").ToString() + " (%s)"),
						    Lua.lua_tostring(L, -1)));
					}
				}
			}
			LuaAPI.lua_settop(L, 0);  /* clear stack */
			LuaConf.fputs(LuaConf.CharPtr.toCharPtr("\n"), LuaConf.stdout);
			LuaConf.fflush(LuaConf.stdout);
			progname = oldprogname;
		}

		static int handle_script(LuaState.lua_State L, string[] argv, int n)
		{
			int status;
			LuaConf.CharPtr fname;
			int narg = getargs(L, argv, n);  /* collect arguments */
			Lua.lua_setglobal(L, LuaConf.CharPtr.toCharPtr("arg"));
			fname = LuaConf.CharPtr.toCharPtr(argv[n]);
			if (LuaConf.strcmp(fname, LuaConf.CharPtr.toCharPtr("-")) == 0 && LuaConf.strcmp(LuaConf.CharPtr.toCharPtr(argv[n - 1]), LuaConf.CharPtr.toCharPtr("--")) != 0)
			{
				fname = null;  /* stdin */
			}
			status = LuaAuxLib.luaL_loadfile(L, fname);
			LuaAPI.lua_insert(L, -(narg + 1));
			if (status == 0)
			{
				status = docall(L, narg, 0);
			}
			else
			{
				Lua.lua_pop(L, narg);
			}
			return report(L, status);
		}

		/* check that argument has no extra characters at the end */
		//#define notail(x)	{if ((x)[2] != '\0') return -1;}

		static int collectargs(string[] argv, /*ref*/ int[] pi, /*ref*/ int[] pv, /*ref*/ int[] pe)
		{
			int i;
			for (i = 1; i < argv.Length; i++)
			{
				if (argv[i][0] != '-')  /* not an option? */
				{
					return i;
				}
				char ch = argv[i][1];
				switch (ch)
				{  
					/* option */
					case '-':
						{
							if (argv[i].Length != 2) 
							{
								return -1;
							}
							return (i + 1) >= argv.Length ? i + 1 : 0;
						}
					case '\0':
						{
							return i;
						}
					case 'i':
						{
							if (argv[i].Length != 2) 
							{
								return -1;
							}
							pi[0] = 1;
							if (argv[i].Length != 2) 
							{
								return -1;
							}
							pv[0] = 1;
							break;
						}
					case 'v':
						{
							if (argv[i].Length != 2) return -1;
							pv[0] = 1;
							break;
						}
					case 'e':
						{
							pe[0] = 1;
							if (argv[i].Length == 2)
							{
								i++;
								if (argv[i] == null) 
								{
									return -1;
								}
							}
							break;
						}
					case 'l':
						{
							if (argv[i].Length == 2)
							{
								i++;
								if (i >= argv.Length) 
								{
									return -1;
								}
							}
							break;
						}
					default: 
						{
							return -1;  /* invalid option */
						}
				}
			}
			return 0;
		}

		static int runargs(LuaState.lua_State L, string[] argv, int n)
		{
			int i;
			for (i = 1; i < n; i++)
			{
				if (argv[i] == null) 
				{
					continue;
				}
				LuaLimits.lua_assert(argv[i][0] == '-');
				char ch = argv[i][1];
				switch (ch)
				{  
					/* option */
					case 'e':
						{
							string chunk = argv[i].Substring(2);
							if (chunk.Equals("")) 
							{
								chunk = argv[++i];
							}
							LuaLimits.lua_assert(chunk != null);
							if (dostring(L, LuaConf.CharPtr.toCharPtr(chunk), LuaConf.CharPtr.toCharPtr("=(command line)")) != 0)
							{
								return 1;
							}
							break;
						}
					case 'l':
						{
							string filename = argv[i].Substring(2);
							if (filename.Equals("")) 
							{
								filename = argv[++i];
							}
							LuaLimits.lua_assert(filename != null);
							if (dolibrary(L, LuaConf.CharPtr.toCharPtr(filename)) != 0)
							{
								return 1;  /* stop if file fails */
							}
							break;
						}
					default: 
						{
							break;
						}
				}
			}
			return 0;
		}

		static int handle_luainit(LuaState.lua_State L)
		{
			LuaConf.CharPtr init = LuaConf.getenv(LuaConf.CharPtr.toCharPtr(LuaConf.LUA_INIT));
			if (LuaConf.CharPtr.isEqual(init, null)) 
			{
				return 0;  /* status OK */
			}
			else if (init.get(0) == '@')
			{
				return dofile(L, LuaConf.CharPtr.plus(init, 1));
			}
			else
			{
				return dostring(L, init, LuaConf.CharPtr.toCharPtr("=" + LuaConf.LUA_INIT));
			}
		}

	    public class Smain
	    {
	        public int argc;
	        public string[] argv;
	        public int status;
	    }
		
		static int pmain(LuaState.lua_State L)
		{
			Smain s = (Smain)LuaAPI.lua_touserdata(L, 1);
			string[] argv = s.argv;
			int script;
			int[] has_i = new int[1];
			int[] has_v = new int[1];
			int[] has_e = new int[1];
			has_i[0] = 0;
			has_v[0] = 0;
			has_e[0] = 0;
			globalL = L;
			if ((argv.Length > 0) && (!argv[0].Equals(""))) 
			{
				progname = LuaConf.CharPtr.toCharPtr(argv[0]);
			}
			LuaAPI.lua_gc(L, Lua.LUA_GCSTOP, 0);  /* stop collector during initialization */
			LuaInit.luaL_openlibs(L);  /* open libraries */
			LuaAPI.lua_gc(L, Lua.LUA_GCRESTART, 0);
			s.status = handle_luainit(L);
			if (s.status != 0) 
			{
				return 0;
			}
			script = collectargs(argv, /*ref*/ has_i, /*ref*/ has_v, /*ref*/ has_e);
			if (script < 0)
			{  
				/* invalid args? */
				print_usage();
				s.status = 1;
				return 0;
			}
			if (has_v[0] != 0)
			{
				print_version();
			}
			s.status = runargs(L, argv, (script > 0) ? script : s.argc);
			if (s.status != 0) 
			{
				return 0;
			}
			if (script != 0)
			{
				s.status = handle_script(L, argv, script);
			}
			if (s.status != 0) 
			{
				return 0;
			}
			if (has_i[0] != 0)
			{
				dotty(L);
			}
			else if ((script == 0) && (has_e[0] == 0) && (has_v[0] == 0))
			{
				if (LuaConf.lua_stdin_is_tty() != 0)
				{
					print_version();
					dotty(L);
				}
				else 
				{
					dofile(L, null);  /* executes stdin as a file */
				}
			}
			return 0;
		}

		public class pmain_delegate : Lua.lua_CFunction
		{
			public int exec(LuaState.lua_State L)
			{
				return pmain(L);
			}
		}
		
		public class traceback_delegate : Lua.lua_CFunction
		{
			public int exec(LuaState.lua_State L)
			{
				return traceback(L);
			}
		}
		
		public static int MainLua(string[] args)
		{
			// prepend the exe name to the arg list as it's done in C
			// so that we don't have to change any of the args indexing
			// code above
			string[] newargs = new string[(args != null ? args.Length : 0) + 1];
			newargs[0] = "lua";//Assembly.GetExecutingAssembly().Location);
			for (int idx = 0; idx < args.Length; idx++)
			{
				newargs[idx + 1] = args[idx];
			}
			args = newargs;

			int status;
			Smain s = new Smain();
			LuaState.lua_State L = Lua.lua_open();  /* create state */
			if (L == null)
			{
				l_message(LuaConf.CharPtr.toCharPtr(args[0]), LuaConf.CharPtr.toCharPtr("cannot create state: not enough memory"));
				return LuaConf.EXIT_FAILURE;
			}
			s.argc = args.Length;
			s.argv = args;
			status = LuaAPI.lua_cpcall(L, new pmain_delegate(), s);
			report(L, status);
			LuaState.lua_close(L);
			return (status != 0) || (s.status != 0) ? LuaConf.EXIT_FAILURE : LuaConf.EXIT_SUCCESS;
		}
	}
}
