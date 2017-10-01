/*
 ** $Id: luac.c,v 1.54 2006/06/02 17:37:11 lhf Exp $
 ** Lua compiler (saves bytecodes to files; also list bytecodes)
 ** See Copyright Notice in lua.h
 */
using System;

namespace kurumi
{
	//using Instruction = System.UInt32;

	public class LuacProgram
	{
		//#include <errno.h>
		//#include <stdio.h>
		//#include <stdlib.h>
		//#include <string.h>

		//#define luac_c
		//#define LUA_CORE

		//#include "lua.h"
		//#include "lauxlib.h"

		//#include "ldo.h"
		//#include "lfunc.h"
		//#include "lmem.h"
		//#include "lobject.h"
		//#include "lopcodes.h"
		//#include "lstring.h"
		//#include "lundump.h"

		static CLib.CharPtr PROGNAME = CLib.CharPtr.toCharPtr("luac");		/* default program name */
		static CLib.CharPtr OUTPUT = CLib.CharPtr.toCharPtr(PROGNAME + ".out"); /* default output file */

		static int listing = 0;			/* list bytecodes? */
		static int dumping = 1;			/* dump bytecodes? */
		static int stripping = 0;			/* strip debug information? */
		static CLib.CharPtr Output = OUTPUT;	/* default output file name */
		static CLib.CharPtr output = Output;	/* actual output file name */
		static CLib.CharPtr progname = PROGNAME;	/* actual program name */

		static void fatal(CLib.CharPtr message)
		{
			CLib.fprintf(CLib.stderr, CLib.CharPtr.toCharPtr("%s: %s\n"), progname, message);
			Environment.Exit(CLib.EXIT_FAILURE);
		}

		static void cannot(CLib.CharPtr what)
		{
			CLib.fprintf(CLib.stderr, CLib.CharPtr.toCharPtr("%s: cannot %s %s: %s\n"), progname, what, output, CLib.strerror(CLib.errno()));
			Environment.Exit(CLib.EXIT_FAILURE);
		}

		static void usage(CLib.CharPtr message)
		{
			if (message.get(0) == '-')			
			{
				CLib.fprintf(CLib.stderr, 
					CLib.CharPtr.toCharPtr("%s: unrecognized option " + LuaConf.getLUA_QS() + "\n"), 
					progname, message);
			}
			else
			{
				CLib.fprintf(CLib.stderr, CLib.CharPtr.toCharPtr("%s: %s\n"), 
				    progname, message);
			}
			CLib.fprintf(CLib.stderr,
			                CLib.CharPtr.toCharPtr("usage: %s [options] [filenames].\n" +
			                "Available options are:\n" +
			                "  -        process stdin\n" +
			                "  -l       list\n" +
			                "  -o name  output to file " + LuaConf.LUA_QL("name") + " (default is \"%s\")\n" +
			                "  -p       parse only\n" +
			                "  -s       strip debug information\n" +
			                "  -v       show version information\n" +
			                "  --       stop handling options\n"),
			                progname, Output);
			Environment.Exit(CLib.EXIT_FAILURE);
		}

		//#define	IS(s)	(strcmp(argv[i],s)==0)

		static int doargs(int argc, string[] argv)
		{
			int i;
			int version = 0;
			if ((argv.Length > 0) && (!argv[0].Equals(""))) 
			{
				progname = CLib.CharPtr.toCharPtr(argv[0]);
			}
			for (i = 1; i < argc; i++)
			{
				if (argv[i][0] != '-')			/* end of options; keep it */
				{
					break;
				}
				else if (CLib.strcmp(CLib.CharPtr.toCharPtr(argv[i]), CLib.CharPtr.toCharPtr("--")) == 0)			/* end of options; skip it */
				{
					++i;
					if (version != 0) 
					{
						++version;
					}
					break;
				}
				else if (CLib.strcmp(CLib.CharPtr.toCharPtr(argv[i]), CLib.CharPtr.toCharPtr("-")) == 0)			/* end of options; use stdin */
				{
					break;
				}
				else if (CLib.strcmp(CLib.CharPtr.toCharPtr(argv[i]), CLib.CharPtr.toCharPtr("-l")) == 0)			/* list */
				{
					++listing;
				}
				else if (CLib.strcmp(CLib.CharPtr.toCharPtr(argv[i]), CLib.CharPtr.toCharPtr("-o")) == 0)			/* output file */
				{
					output = CLib.CharPtr.toCharPtr(argv[++i]);
					if (CLib.CharPtr.isEqual(output, null) || (output.get(0) == 0)) 
					{
						usage(CLib.CharPtr.toCharPtr(LuaConf.LUA_QL("-o") + " needs argument"));
					}
					if (CLib.strcmp(CLib.CharPtr.toCharPtr(argv[i]), CLib.CharPtr.toCharPtr("-")) == 0) 
					{
						output = null;
					}
				}
				else if (CLib.strcmp(CLib.CharPtr.toCharPtr(argv[i]), CLib.CharPtr.toCharPtr("-p")) == 0)			/* parse only */
				{
					dumping = 0;
				}
				else if (CLib.strcmp(CLib.CharPtr.toCharPtr(argv[i]), CLib.CharPtr.toCharPtr("-s")) == 0)			/* strip debug information */
				{
					stripping = 1;
				}
				else if (CLib.strcmp(CLib.CharPtr.toCharPtr(argv[i]), CLib.CharPtr.toCharPtr("-v")) == 0)			/* show version */
				{
					++version;
				}
				else					/* unknown option */
				{
					usage(CLib.CharPtr.toCharPtr(argv[i]));
				}
			}
			if (i == argc && ((listing != 0) || (dumping == 0)))
			{
				dumping = 0;
				argv[--i] = Output.ToString();
			}
			if (version != 0)
			{
				CLib.printf(CLib.CharPtr.toCharPtr("%s  %s\n"), Lua.LUA_RELEASE, Lua.LUA_COPYRIGHT);
				if (version == argc - 1) 
				{
					Environment.Exit(CLib.EXIT_SUCCESS);
				}
			}
			return i;
		}

		static LuaObject.Proto toproto(LuaState.lua_State L, int i) 
		{
			return LuaObject.clvalue(LuaObject.TValue.plus(L.top, i)).l.p;
		}

		static LuaObject.Proto combine(LuaState.lua_State L, int n)
		{
			if (n == 1)
			{
				return toproto(L, -1);
			}
			else
			{
				int i, pc;
				LuaObject.Proto f = LuaFunc.luaF_newproto(L);
				LuaObject.setptvalue2s(L, L.top,f); 
				LuaDo.incr_top(L);
				f.source = LuaString.luaS_newliteral(L, CLib.CharPtr.toCharPtr("=(" + PROGNAME + ")"));
				f.maxstacksize = 1;
				pc = 2 * n + 1;
                /*UInt32[]*/
                /*Instruction[]*/
                /*UInt32*/
                /*Instruction*/
				f.code = (long[])LuaMem.luaM_newvector_long(L, pc, new ClassType(ClassType.TYPE_LONG));
				f.sizecode = pc;
				f.p = LuaMem.luaM_newvector_Proto(L, n, new ClassType(ClassType.TYPE_PROTO));
				f.sizep = n;
				pc = 0;
				for (i = 0; i < n; i++)
				{
					f.p[i] = toproto(L,i-n-1);
					f.code[pc++] = (long/*uint*/)LuaOpCodes.CREATE_ABx(LuaOpCodes.OpCode.OP_CLOSURE, 0, i);
					f.code[pc++] = (long/*uint*/)LuaOpCodes.CREATE_ABC(LuaOpCodes.OpCode.OP_CALL, 0, 1, 1);
				}
				f.code[pc++] = (long/*uint*/)LuaOpCodes.CREATE_ABC(LuaOpCodes.OpCode.OP_RETURN, 0, 1, 0);
				return f;
			}
		}

        //FIXME:StreamProxy/*object*/ u
		static int writer(LuaState.lua_State L, CLib.CharPtr p, int/*uint*/ size, object u)
		{
			//UNUSED(L);
			return ((CLib.fwrite(p, (int)size, 1, (StreamProxy)u) != 1) && (size != 0)) ? 1 : 0;
		}
		
		public class writer_delegate : Lua.lua_Writer
		{
			public int exec(LuaState.lua_State L, CLib.CharPtr p, int/*uint*/ sz, object ud)
			{
                //FIXME:StreamProxy/*object*/ u
				return writer(L, p, sz, ud);
			}
		}

	    public class SmainLuac
	    {
	        public int argc;
	        public string[] argv;
	        public int status;
	    }
		
		static int pmain(LuaState.lua_State L)
		{
			SmainLuac s = (SmainLuac)LuaAPI.lua_touserdata(L, 1);
			int argc = s.argc;
			string[] argv = s.argv;
			LuaObject.Proto f;
			int i;
			if (LuaAPI.lua_checkstack(L,argc) == 0) 
			{
				fatal(CLib.CharPtr.toCharPtr("too many input files"));
			}
			for (i = 0; i < argc; i++)
			{
				CLib.CharPtr filename = (CLib.strcmp(CLib.CharPtr.toCharPtr(argv[i]), CLib.CharPtr.toCharPtr("-")) == 0) ? null : CLib.CharPtr.toCharPtr(argv[i]);
				if (LuaAuxLib.luaL_loadfile(L,filename) != 0) 
				{
					fatal(Lua.lua_tostring(L,-1));
				}
			}
			f = combine(L, argc);
			if (listing != 0) 
			{
				LuaPrint.luaU_print(f,(listing > 1) ? 1 : 0);
			}
			if (dumping!=0)
			{
				StreamProxy D = (CLib.CharPtr.isEqual(output, null)) ? CLib.stdout : CLib.fopen(output, CLib.CharPtr.toCharPtr("wb"));
                if (D == null)
                {
                    cannot(CLib.CharPtr.toCharPtr("open"));
                }
                LuaLimits.lua_lock(L);
                LuaDump.luaU_dump(L, f, new writer_delegate(), D, stripping);
				LuaLimits.lua_unlock(L);
                if (CLib.ferror(D) != 0)
                {
                    cannot(CLib.CharPtr.toCharPtr("write"));
                }
                if (CLib.fclose(D) != 0)
                {
                    cannot(CLib.CharPtr.toCharPtr("close"));
                }
			}
			return 0;
		}

		public static int MainLuac(string[] args)
		{
			// prepend the exe name to the arg list as it's done in C
			// so that we don't have to change any of the args indexing
			// code above
			string[] newargs = new string[(args != null ? args.Length : 0) + 1];
			newargs[0] = "luac";//Assembly.GetExecutingAssembly().Location);
			for (int idx = 0; idx < args.Length; idx++)
			{
				newargs[idx + 1] = args[idx];
			}
			args = newargs;

			LuaState.lua_State L;
			SmainLuac s = new SmainLuac();
			int argc = args.Length;
			int i = doargs(argc,args);
			//newargs.RemoveRange(0, i);
			string[] newargs2 = new string[newargs.Length - i];
			for (int idx = newargs.Length - i; idx < newargs.Length; idx++)
			{
				newargs2[idx - (newargs.Length - i)] = newargs[idx];
			}
			argc -= i;
			args = newargs2;//(string[])newargs.ToArray();
			if (argc <= 0) 
			{
				usage(CLib.CharPtr.toCharPtr("no input files given"));
			}
			L = Lua.lua_open();
			if (L == null) 
			{
				fatal(CLib.CharPtr.toCharPtr("not enough memory for state"));
			}
			s.argc = argc;
			s.argv = args;
			if (LuaAPI.lua_cpcall(L, new pmain_delegate(), s) != 0)
			{
				fatal(Lua.lua_tostring(L,-1));
			}
			LuaState.lua_close(L);
			return CLib.EXIT_SUCCESS;
		}
		
		public class pmain_delegate : Lua.lua_CFunction
		{
			public int exec(LuaState.lua_State L)
			{
				return pmain(L);
			}
		}
	}
}
