package kurumi;

//
// ** $Id: luac.c,v 1.54 2006/06/02 17:37:11 lhf Exp $
// ** Lua compiler (saves bytecodes to files; also list bytecodes)
// ** See Copyright Notice in lua.h
// 

//using Instruction = System.UInt32;

public class LuacProgram {
	///#include <errno.h>
	///#include <stdio.h>
	///#include <stdlib.h>
	///#include <string.h>

	///#define luac_c
	///#define LUA_CORE

	///#include "lua.h"
	///#include "lauxlib.h"

	///#include "ldo.h"
	///#include "lfunc.h"
	///#include "lmem.h"
	///#include "lobject.h"
	///#include "lopcodes.h"
	///#include "lstring.h"
	///#include "lundump.h"

	private static LuaConf.CharPtr PROGNAME = LuaConf.CharPtr.toCharPtr("luac"); // default program name 
	private static LuaConf.CharPtr OUTPUT = LuaConf.CharPtr.toCharPtr(PROGNAME + ".out"); // default output file 

	private static int listing = 0; // list bytecodes? 
	private static int dumping = 1; // dump bytecodes? 
	private static int stripping = 0; // strip debug information? 
	private static LuaConf.CharPtr Output = OUTPUT; // default output file name 
	private static LuaConf.CharPtr output = Output; // actual output file name 
	private static LuaConf.CharPtr progname = PROGNAME; // actual program name 

	private static void fatal(LuaConf.CharPtr message) {
		LuaConf.fprintf(LuaConf.stderr, LuaConf.CharPtr.toCharPtr("%s: %s\n"), progname, message);
		System.exit(LuaConf.EXIT_FAILURE);
	}

	private static void cannot(LuaConf.CharPtr what) {
		LuaConf.fprintf(LuaConf.stderr, LuaConf.CharPtr.toCharPtr("%s: cannot %s %s: %s\n"), progname, what, output, LuaConf.strerror(LuaConf.errno()));
		System.exit(LuaConf.EXIT_FAILURE);
	}

	private static void usage(LuaConf.CharPtr message) {
		if (message.get(0) == '-') {
			LuaConf.fprintf(LuaConf.stderr, LuaConf.CharPtr.toCharPtr("%s: unrecognized option " + LuaConf.getLUA_QS() + "\n"), progname, message);
		}
		else {
			LuaConf.fprintf(LuaConf.stderr, LuaConf.CharPtr.toCharPtr("%s: %s\n"), progname, message);
		}
		LuaConf.fprintf(LuaConf.stderr, LuaConf.CharPtr.toCharPtr("usage: %s [options] [filenames].\n" + "Available options are:\n" + "  -        process stdin\n" + "  -l       list\n" + "  -o name  output to file " + LuaConf.LUA_QL("name") + " (default is \"%s\")\n" + "  -p       parse only\n" + "  -s       strip debug information\n" + "  -v       show version information\n" + "  --       stop handling options\n"), progname, Output);
		System.exit(LuaConf.EXIT_FAILURE);
	}

	///#define	IS(s)	(strcmp(argv[i],s)==0)

	private static int doargs(int argc, String[] argv) {
		int i;
		int version = 0;
		if ((argv.length > 0) && (!argv[0].equals(""))) {
			progname = LuaConf.CharPtr.toCharPtr(argv[0]);
		}
		for (i = 1; i < argc; i++) {
			if (argv[i].charAt(0) != '-') { // end of options; keep it 
				break;
			}
			else if (LuaConf.strcmp(LuaConf.CharPtr.toCharPtr(argv[i]), LuaConf.CharPtr.toCharPtr("--")) == 0) { // end of options; skip it 
				++i;
				if (version != 0) {
					++version;
				}
				break;
			}
			else if (LuaConf.strcmp(LuaConf.CharPtr.toCharPtr(argv[i]), LuaConf.CharPtr.toCharPtr("-")) == 0) { // end of options; use stdin 
				break;
			}
			else if (LuaConf.strcmp(LuaConf.CharPtr.toCharPtr(argv[i]), LuaConf.CharPtr.toCharPtr("-l")) == 0) { // list 
				++listing;
			}
			else if (LuaConf.strcmp(LuaConf.CharPtr.toCharPtr(argv[i]), LuaConf.CharPtr.toCharPtr("-o")) == 0) { // output file 
				output = LuaConf.CharPtr.toCharPtr(argv[++i]);
				if (LuaConf.CharPtr.isEqual(output, null) || (output.get(0) == 0)) {
					usage(LuaConf.CharPtr.toCharPtr(LuaConf.LUA_QL("-o") + " needs argument"));
				}
				if (LuaConf.strcmp(LuaConf.CharPtr.toCharPtr(argv[i]), LuaConf.CharPtr.toCharPtr("-")) == 0) {
					output = null;
				}
			}
			else if (LuaConf.strcmp(LuaConf.CharPtr.toCharPtr(argv[i]), LuaConf.CharPtr.toCharPtr("-p")) == 0) { // parse only 
				dumping = 0;
			}
			else if (LuaConf.strcmp(LuaConf.CharPtr.toCharPtr(argv[i]), LuaConf.CharPtr.toCharPtr("-s")) == 0) { // strip debug information 
				stripping = 1;
			}
			else if (LuaConf.strcmp(LuaConf.CharPtr.toCharPtr(argv[i]), LuaConf.CharPtr.toCharPtr("-v")) == 0) { // show version 
				++version;
			}
			else { // unknown option 
				usage(LuaConf.CharPtr.toCharPtr(argv[i]));
			}
		}
		if (i == argc && ((listing != 0) || (dumping == 0))) {
			dumping = 0;
			argv[--i] = Output.toString();
		}
		if (version != 0) {
			LuaConf.printf(LuaConf.CharPtr.toCharPtr("%s  %s\n"), Lua.LUA_RELEASE, Lua.LUA_COPYRIGHT);
			if (version == argc - 1) {
				System.exit(LuaConf.EXIT_SUCCESS);
			}
		}
		return i;
	}

	private static LuaObject.Proto toproto(LuaState.lua_State L, int i) {
		return LuaObject.clvalue(LuaObject.TValue.plus(L.top, i)).l.p;
	}

	private static LuaObject.Proto combine(LuaState.lua_State L, int n) {
		if (n == 1) {
			return toproto(L, -1);
		}
		else {
			int i, pc;
			LuaObject.Proto f = LuaFunc.luaF_newproto(L);
			LuaObject.setptvalue2s(L, L.top, f);
			LuaDo.incr_top(L);
			f.source = LuaString.luaS_newliteral(L, LuaConf.CharPtr.toCharPtr("=(" + PROGNAME + ")"));
			f.maxstacksize = 1;
			pc = 2 * n + 1;
			//UInt32[]
			//Instruction[]
			//UInt32
			//Instruction
			f.code = (long[])LuaMem.luaM_newvector_long(L, pc, new ClassType(ClassType.TYPE_LONG));
			f.sizecode = pc;
			f.p = LuaMem.luaM_newvector_Proto(L, n, new ClassType(ClassType.TYPE_PROTO));
			f.sizep = n;
			pc = 0;
			for (i = 0; i < n; i++) {
				f.p[i] = toproto(L, i-n-1);
				f.code[pc++] = (long)LuaOpCodes.CREATE_ABx(LuaOpCodes.OpCode.OP_CLOSURE, 0, i); //uint
				f.code[pc++] = (long)LuaOpCodes.CREATE_ABC(LuaOpCodes.OpCode.OP_CALL, 0, 1, 1); //uint
			}
			f.code[pc++] = (long)LuaOpCodes.CREATE_ABC(LuaOpCodes.OpCode.OP_RETURN, 0, 1, 0); //uint
			return f;
		}
	}

	//FIXME:StreamProxy/*object*/ u
	private static int writer(LuaState.lua_State L, LuaConf.CharPtr p, int size, Object u) { //uint
		//UNUSED(L);
		return ((LuaConf.fwrite(p, (int)size, 1, (StreamProxy)u) != 1) && (size != 0)) ? 1 : 0;
	}

	public static class writer_delegate implements Lua.lua_Writer {
		public final int exec(LuaState.lua_State L, LuaConf.CharPtr p, int sz, Object ud) { //uint
			//FIXME:StreamProxy/*object*/ u
			return writer(L, p, sz, ud);
		}
	}

    public static class Smain
    {
        public int argc;
		public String[] argv;
        public int status;
    }
	
	private static int pmain(LuaState.lua_State L) {
		Smain s = (Smain)LuaAPI.lua_touserdata(L, 1);
		int argc = s.argc;
		String[] argv = s.argv;
		LuaObject.Proto f;
		int i;
		if (LuaAPI.lua_checkstack(L, argc) == 0) {
			fatal(LuaConf.CharPtr.toCharPtr("too many input files"));
		}
		for (i = 0; i < argc; i++) {
			LuaConf.CharPtr filename = (LuaConf.strcmp(LuaConf.CharPtr.toCharPtr(argv[i]), LuaConf.CharPtr.toCharPtr("-")) == 0) ? null : LuaConf.CharPtr.toCharPtr(argv[i]);
			if (LuaAuxLib.luaL_loadfile(L, filename) != 0) {
				fatal(Lua.lua_tostring(L, -1));
			}
		}
		f = combine(L, argc);
		if (listing != 0) {
			LuaPrint.luaU_print(f, (listing > 1) ? 1 : 0);
		}
		if (dumping!=0) {
			StreamProxy D = (LuaConf.CharPtr.isEqual(output, null)) ? LuaConf.stdout : LuaConf.fopen(output, LuaConf.CharPtr.toCharPtr("wb"));
			if (D == null) {
				cannot(LuaConf.CharPtr.toCharPtr("open"));
			}
			LuaLimits.lua_lock(L);
			LuaDump.luaU_dump(L, f, new writer_delegate(), D, stripping);
			LuaLimits.lua_unlock(L);
			if (LuaConf.ferror(D) != 0) {
				cannot(LuaConf.CharPtr.toCharPtr("write"));
			}
			if (LuaConf.fclose(D) != 0) {
				cannot(LuaConf.CharPtr.toCharPtr("close"));
			}
		}
		return 0;
	}

	public static int MainLuac(String[] args) {
		// prepend the exe name to the arg list as it's done in C
		// so that we don't have to change any of the args indexing
		// code above
		String[] newargs = new String[(args != null ? args.length : 0) + 1];
		newargs[0] = "luac"; //Assembly.GetExecutingAssembly().Location);
		for (int idx = 0; idx < args.length; idx++) {
			newargs[idx + 1] = args[idx];
		}
		args = newargs;

		LuaState.lua_State L;
		Smain s = new Smain();
		int argc = args.length;
		int i = doargs(argc, args);
		//newargs.RemoveRange(0, i);
		String[] newargs2 = new String[newargs.length - i];
		for (int idx = newargs.length - i; idx < newargs.length; idx++) {
			newargs2[idx - (newargs.length - i)] = newargs[idx];
		}
		argc -= i;
		args = newargs2; //(string[])newargs.ToArray();
		if (argc <= 0) {
			usage(LuaConf.CharPtr.toCharPtr("no input files given"));
		}
		L = Lua.lua_open();
		if (L == null) {
			fatal(LuaConf.CharPtr.toCharPtr("not enough memory for state"));
		}
		s.argc = argc;
		s.argv = args;
		if (LuaAPI.lua_cpcall(L, new pmain_delegate(), s) != 0) {
			fatal(Lua.lua_tostring(L, -1));
		}
		LuaState.lua_close(L);
		return LuaConf.EXIT_SUCCESS;
	}

	public static class pmain_delegate implements Lua.lua_CFunction {
		public final int exec(LuaState.lua_State L) {
			return pmain(L);
		}
	}
}