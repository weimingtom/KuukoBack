package kurumi;

//
// ** $Id: luaconf.h,v 1.82.1.7 2008/02/11 16:25:08 roberto Exp $
// ** Configuration file for Lua
// ** See Copyright Notice in lua.h
// 

//using LUA_INTEGER = System.Int32;
//using LUA_NUMBER = System.Double;
//using LUAI_UACNUMBER = System.Double;
//using LUA_INTFRM_T = System.Int64;
//using TValue = Lua.TValue;
//using lua_Number = System.Double;

public class LuaConf {
	public static final boolean CATCH_EXCEPTIONS = false;

//        
//		 ** ==================================================================
//		 ** Search for "@@" to find all configurable definitions.
//		 ** ===================================================================
//		 


//        
//		@@ LUA_ANSI controls the use of non-ansi features.
//		 ** CHANGE it (define it) if you want Lua to avoid the use of any
//		 ** non-ansi feature or library.
//		 
	///#if defined(__STRICT_ANSI__)
	///#define LUA_ANSI
	///#endif


	///#if !defined(LUA_ANSI) && _WIN32
	///#define LUA_WIN
	///#endif

	///#if defined(LUA_USE_LINUX)
	///#define LUA_USE_POSIX
	///#define LUA_USE_DLOPEN		/* needs an extra library: -ldl */
	///#define LUA_USE_READLINE	/* needs some extra libraries */
	///#endif

	///#if defined(LUA_USE_MACOSX)
	///#define LUA_USE_POSIX
	///#define LUA_DL_DYLD		/* does not need extra library */
	///#endif



//        
//		@@ LUA_USE_POSIX includes all functionallity listed as X/Open System
//		@* Interfaces Extension (XSI).
//		 ** CHANGE it (define it) if your system is XSI compatible.
//		 
	///#if defined(LUA_USE_POSIX)
	///#define LUA_USE_MKSTEMP
	///#define LUA_USE_ISATTY
	///#define LUA_USE_POPEN
	///#define LUA_USE_ULONGJMP
	///#endif


//        
//		@@ LUA_PATH and LUA_CPATH are the names of the environment variables that
//		@* Lua check to set its paths.
//		@@ LUA_INIT is the name of the environment variable that Lua
//		@* checks for initialization code.
//		 ** CHANGE them if you want different names.
//		 
	public static final String LUA_PATH = "LUA_PATH";
	public static final String LUA_CPATH = "LUA_CPATH";
	public static final String LUA_INIT = "LUA_INIT";


//        
//		@@ LUA_PATH_DEFAULT is the default path that Lua uses to look for
//		@* Lua libraries.
//		@@ LUA_CPATH_DEFAULT is the default path that Lua uses to look for
//		@* C libraries.
//		 ** CHANGE them if your machine has a non-conventional directory
//		 ** hierarchy or if you want to install your libraries in
//		 ** non-conventional directories.
//		 
	///#if _WIN32
//        
//		 ** In Windows, any exclamation mark ('!') in the path is replaced by the
//		 ** path of the directory of the executable file of the current process.
//		 
	public static final String LUA_LDIR = "!\\lua\\";
	public static final String LUA_CDIR = "!\\";
	public static final String LUA_PATH_DEFAULT = ".\\?.lua;" + LUA_LDIR + "?.lua;" + LUA_LDIR + "?\\init.lua;" + LUA_CDIR + "?.lua;" + LUA_CDIR + "?\\init.lua";
	public static final String LUA_CPATH_DEFAULT = ".\\?.dll;" + LUA_CDIR + "?.dll;" + LUA_CDIR + "loadall.dll";

	///#else
//		public const string LUA_ROOT	= "/usr/local/";
//		public const string LUA_LDIR	= LUA_ROOT + "share/lua/5.1/";
//		public const string LUA_CDIR	= LUA_ROOT + "lib/lua/5.1/";
//		public const string LUA_PATH_DEFAULT  =
//			"./?.lua;"  + LUA_LDIR + "?.lua;"  + LUA_LDIR + "?/init.lua;" +
//			LUA_CDIR + "?.lua;"  + LUA_CDIR + "?/init.lua";
//		public const string LUA_CPATH_DEFAULT =
//			"./?.so;"  + LUA_CDIR + "?.so;" + LUA_CDIR + "loadall.so";
	///#endif


//        
//		@@ LUA_DIRSEP is the directory separator (for submodules).
//		 ** CHANGE it if your machine does not use "/" as the directory separator
//		 ** and is not Windows. (On Windows Lua automatically uses "\".)
//		 
	///#if _WIN32
	public static final String LUA_DIRSEP = "\\";
	///#else
//		public const string LUA_DIRSEP = "/";
	///#endif


//        
//		@@ LUA_PATHSEP is the character that separates templates in a path.
//		@@ LUA_PATH_MARK is the string that marks the substitution points in a
//		@* template.
//		@@ LUA_EXECDIR in a Windows path is replaced by the executable's
//		@* directory.
//		@@ LUA_IGMARK is a mark to ignore all before it when bulding the
//		@* luaopen_ function name.
//		 ** CHANGE them if for some reason your system cannot use those
//		 ** characters. (E.g., if one of those characters is a common character
//		 ** in file/directory names.) Probably you do not need to change them.
//		 
	public static final String LUA_PATHSEP = ";";
	public static final String LUA_PATH_MARK = "?";
	public static final String LUA_EXECDIR = "!";
	public static final String LUA_IGMARK = "-";


//        
//		@@ LUA_INTEGER is the integral type used by lua_pushinteger/lua_tointeger.
//		 ** CHANGE that if ptrdiff_t is not adequate on your machine. (On most
//		 ** machines, ptrdiff_t gives a good choice between int or long.)
//		 
	///#define LUA_INTEGER	ptrdiff_t


//        
//		@@ LUA_API is a mark for all core API functions.
//		@@ LUALIB_API is a mark for all standard library functions.
//		 ** CHANGE them if you need to define those functions in some special way.
//		 ** For instance, if you want to create one Windows DLL with the core and
//		 ** the libraries, you may want to use the following definition (define
//		 ** LUA_BUILD_AS_DLL to get it).
//		 
	///#if LUA_BUILD_AS_DLL

	///#if defined(LUA_CORE) || defined(LUA_LIB)
	///#define LUA_API __declspec(dllexport)
	///#else
	///#define LUA_API __declspec(dllimport)
	///#endif

	///#else

	///#define LUA_API		extern

	///#endif

	// more often than not the libs go together with the core 
	///#define LUALIB_API	LUA_API


//        
//		@@ LUAI_FUNC is a mark for all extern functions that are not to be
//		@* exported to outside modules.
//		@@ LUAI_DATA is a mark for all extern (const) variables that are not to
//		@* be exported to outside modules.
//		 ** CHANGE them if you need to mark them in some special way. Elf/gcc
//		 ** (versions 3.2 and later) mark them as "hidden" to optimize access
//		 ** when Lua is compiled as a shared library.
//		 
	///#if defined(luaall_c)
	///#define LUAI_FUNC	static
	///#define LUAI_DATA	/* empty */

	///#elif defined(__GNUC__) && ((__GNUC__*100 + __GNUC_MINOR__) >= 302) && \
	//      defined(__ELF__)
	///#define LUAI_FUNC	__attribute__((visibility("hidden"))) extern
	///#define LUAI_DATA	LUAI_FUNC

	///#else
	///#define LUAI_FUNC	extern
	///#define LUAI_DATA	extern
	///#endif



//        
//		@@ LUA_QL describes how error messages quote program elements.
//		 ** CHANGE it if you want a different appearance.
//		 
	public static CLib.CharPtr LUA_QL(String x) {
		return CLib.CharPtr.toCharPtr("'" + x + "'");
	}

	public static CLib.CharPtr getLUA_QS() {
			return LUA_QL("%s");
	}

//        
//		@@ LUA_IDSIZE gives the maximum size for the description of the source
//		@* of a function in debug information.
//		 ** CHANGE it if you want a different size.
//		 
	public static final int LUA_IDSIZE = 60;

//        
//		 ** {==================================================================
//		 ** Stand-alone configuration
//		 ** ===================================================================
//		 

	///#if lua_c || luaall_c

//        
//		@@ lua_stdin_is_tty detects whether the standard input is a 'tty' (that
//		@* is, whether we're running lua interactively).
//		 ** CHANGE it if you have a better definition for non-POSIX/non-Windows
//		 ** systems.
//		 
	///#if LUA_USE_ISATTY
	///#include <unistd.h>
	///#define lua_stdin_is_tty()	isatty(0)
	///#elif LUA_WIN
	///#include <io.h>
	///#include <stdio.h>
	///#define lua_stdin_is_tty()	_isatty(_fileno(stdin))
	///#else
	public static int lua_stdin_is_tty() {
		return 1;
	} // assume stdin is a tty 
	///#endif

//        
//		@@ LUA_PROMPT is the default prompt used by stand-alone Lua.
//		@@ LUA_PROMPT2 is the default continuation prompt used by stand-alone Lua.
//		 ** CHANGE them if you want different prompts. (You can also change the
//		 ** prompts dynamically, assigning to globals _PROMPT/_PROMPT2.)
//		 
	public static final String LUA_PROMPT = "> ";
	public static final String LUA_PROMPT2 = ">> ";

//        
//		@@ LUA_PROGNAME is the default name for the stand-alone Lua program.
//		 ** CHANGE it if your stand-alone interpreter has a different name and
//		 ** your system is not able to detect that name automatically.
//		 
	public static final String LUA_PROGNAME = "lua";

//        
//		@@ LUA_MAXINPUT is the maximum length for an input line in the
//		@* stand-alone interpreter.
//		 ** CHANGE it if you need longer lines.
//		 
	public static final int LUA_MAXINPUT = 512;

//        
//		@@ lua_readline defines how to show a prompt and then read a line from
//		@* the standard input.
//		@@ lua_saveline defines how to "save" a read line in a "history".
//		@@ lua_freeline defines how to free a line read by lua_readline.
//		 ** CHANGE them if you want to improve this functionality (e.g., by using
//		 ** GNU readline and history facilities).
//		 
	///#if LUA_USE_READLINE
	///#include <stdio.h>
	///#include <readline/readline.h>
	///#include <readline/history.h>
	///#define lua_readline(L,b,p)	((void)L, ((b)=readline(p)) != null)
	///#define lua_saveline(L,idx) \
	//	if (lua_strlen(L,idx) > 0)  /* non-empty line? */ \
	//	  add_history(lua_tostring(L, idx));  /* add it to history */
	///#define lua_freeline(L,b)	((void)L, free(b))
	///#else
	public static boolean lua_readline(LuaState.lua_State L, CLib.CharPtr b, CLib.CharPtr p) {
		CLib.fputs(p, CLib.stdout);
		CLib.fflush(CLib.stdout); // show prompt 
		return (CLib.CharPtr.isNotEqual(CLib.fgets(b, CLib.stdin), null)); // get line 
	}

	public static void lua_saveline(LuaState.lua_State L, int idx) {

	}

	public static void lua_freeline(LuaState.lua_State L, CLib.CharPtr b) {

	}
	///#endif

	///#endif

	// }================================================================== 

//        
//		@@ LUAI_GCPAUSE defines the default pause between garbage-collector cycles
//		@* as a percentage.
//		 ** CHANGE it if you want the GC to run faster or slower (higher values
//		 ** mean larger pauses which mean slower collection.) You can also change
//		 ** this value dynamically.
//		 
	public static final int LUAI_GCPAUSE = 200; // 200% (wait memory to double before next GC) 

//        
//		@@ LUAI_GCMUL defines the default speed of garbage collection relative to
//		@* memory allocation as a percentage.
//		 ** CHANGE it if you want to change the granularity of the garbage
//		 ** collection. (Higher values mean coarser collections. 0 represents
//		 ** infinity, where each step performs a full collection.) You can also
//		 ** change this value dynamically.
//		 
	public static final int LUAI_GCMUL = 200; // GC runs 'twice the speed' of memory allocation 

//        
//		@@ LUA_COMPAT_GETN controls compatibility with old getn behavior.
//		 ** CHANGE it (define it) if you want exact compatibility with the
//		 ** behavior of setn/getn in Lua 5.0.
//		 
	///#undef LUA_COMPAT_GETN /* dotnet port doesn't define in the first place */

//        
//		@@ LUA_COMPAT_LOADLIB controls compatibility about global loadlib.
//		 ** CHANGE it to undefined as soon as you do not need a global 'loadlib'
//		 ** function (the function is still available as 'package.loadlib').
//		 
	///#undef LUA_COMPAT_LOADLIB /* dotnet port doesn't define in the first place */

//        
//		@@ LUA_COMPAT_VARARG controls compatibility with old vararg feature.
//		 ** CHANGE it to undefined as soon as your programs use only '...' to
//		 ** access vararg parameters (instead of the old 'arg' table).
//		 
	///#define LUA_COMPAT_VARARG /* defined higher up */

//        
//		@@ LUA_COMPAT_MOD controls compatibility with old math.mod function.
//		 ** CHANGE it to undefined as soon as your programs use 'math.fmod' or
//		 ** the new '%' operator instead of 'math.mod'.
//		 
	///#define LUA_COMPAT_MOD /* defined higher up */

//        
//		@@ LUA_COMPAT_LSTR controls compatibility with old long string nesting
//		@* facility.
//		 ** CHANGE it to 2 if you want the old behaviour, or undefine it to turn
//		 ** off the advisory error when nesting [[...]].
//		 
	///#define LUA_COMPAT_LSTR		1
	///#define LUA_COMPAT_LSTR /* defined higher up */

//        
//		@@ LUA_COMPAT_GFIND controls compatibility with old 'string.gfind' name.
//		 ** CHANGE it to undefined as soon as you rename 'string.gfind' to
//		 ** 'string.gmatch'.
//		 
	///#define LUA_COMPAT_GFIND /* defined higher up */

//        
//		@@ LUA_COMPAT_OPENLIB controls compatibility with old 'luaL_openlib'
//		@* behavior.
//		 ** CHANGE it to undefined as soon as you replace to 'luaL_register'
//		 ** your uses of 'luaL_openlib'
//		 
	///#define LUA_COMPAT_OPENLIB /* defined higher up */

//        
//		@@ luai_apicheck is the assert macro used by the Lua-C API.
//		 ** CHANGE luai_apicheck if you want Lua to perform some checks in the
//		 ** parameters it gets from API calls. This may slow down the interpreter
//		 ** a bit, but may be quite useful when debugging C code that interfaces
//		 ** with Lua. A useful redefinition is to use assert.h.
//		 
	///#if LUA_USE_APICHECK
//		public static void luai_apicheck(lua_State L, bool o)	
//		{
//			ClassType.Assert(o);
//		}
//		
//		public static void luai_apicheck(lua_State L, int o) 
//		{
//			ClassType.Assert(o != 0);
//		}
	///#else
	public static void luai_apicheck(LuaState.lua_State L, boolean o) {

	}

	public static void luai_apicheck(LuaState.lua_State L, int o) {

	}
	///#endif


//        
//		@@ LUAI_BITSINT defines the number of bits in an int.
//		 ** CHANGE here if Lua cannot automatically detect the number of bits of
//		 ** your machine. Probably you do not need to change this.
//		 
	// avoid overflows in comparison 
	///#if INT_MAX-20 < 32760
	//public const int LUAI_BITSINT	= 16
	///#elif INT_MAX > 2147483640L
	// int has at least 32 bits 
	public static final int LUAI_BITSINT = 32;
	///#else
	///#error "you must define LUA_BITSINT with number of bits in an integer"
	///#endif

//        
//		@@ LUAI_UINT32 is an unsigned integer with at least 32 bits.
//		@@ LUAI_INT32 is an signed integer with at least 32 bits.
//		@@ LUAI_UMEM is an unsigned integer big enough to count the total
//		@* memory used by Lua.
//		@@ LUAI_MEM is a signed integer big enough to count the total memory
//		@* used by Lua.
//		 ** CHANGE here if for some weird reason the default definitions are not
//		 ** good enough for your machine. (The definitions in the 'else'
//		 ** part always works, but may waste space on machines with 64-bit
//		 ** longs.) Probably you do not need to change this.
//		 
	///#if LUAI_BITSINT >= 32
	///#define LUAI_UINT32	unsigned int
	///#define LUAI_INT32	int
	///#define LUAI_MAXINT32	INT_MAX
	///#define LUAI_UMEM	uint
	///#define LUAI_MEM	ptrdiff_t
	///#else
	// 16-bit ints 
	///#define LUAI_UINT32	unsigned long
	///#define LUAI_INT32	long
	///#define LUAI_MAXINT32	LONG_MAX
	///#define LUAI_UMEM	unsigned long
	///#define LUAI_MEM	long
	///#endif


//        
//		@@ LUAI_MAXCALLS limits the number of nested calls.
//		 ** CHANGE it if you need really deep recursive calls. This limit is
//		 ** arbitrary; its only purpose is to stop infinite recursion before
//		 ** exhausting memory.
//		 
	public static final int LUAI_MAXCALLS = 20000;

//        
//		@@ LUAI_MAXCSTACK limits the number of Lua stack slots that a C function
//		@* can use.
//		 ** CHANGE it if you need lots of (Lua) stack space for your C
//		 ** functions. This limit is arbitrary; its only purpose is to stop C
//		 ** functions to consume unlimited stack space. (must be smaller than
//		 ** -LUA_REGISTRYINDEX)
//		 
	public static final int LUAI_MAXCSTACK = 8000;

//        
//		 ** {==================================================================
//		 ** CHANGE (to smaller values) the following definitions if your system
//		 ** has a small C stack. (Or you may want to change them to larger
//		 ** values if your system has a large C stack and these limits are
//		 ** too rigid for you.) Some of these constants control the size of
//		 ** stack-allocated arrays used by the compiler or the interpreter, while
//		 ** others limit the maximum number of recursive calls that the compiler
//		 ** or the interpreter can perform. Values too large may cause a C stack
//		 ** overflow for some forms of deep constructs.
//		 ** ===================================================================
//		 


//        
//		@@ LUAI_MAXCCALLS is the maximum depth for nested C calls (short) and
//		@* syntactical nested non-terminals in a program.
//		 
	public static final int LUAI_MAXCCALLS = 100;//200; //FIXME:

//        
//		@@ LUAI_MAXVARS is the maximum number of local variables per function
//		@* (must be smaller than 250).
//		 
	public static final int LUAI_MAXVARS = 200;

//        
//		@@ LUAI_MAXUPVALUES is the maximum number of upvalues per function
//		@* (must be smaller than 250).
//		 
	public static final int LUAI_MAXUPVALUES = 60;

//        
//		@@ LUAL_BUFFERSIZE is the buffer size used by the lauxlib buffer system.
//		 
	public static final int LUAL_BUFFERSIZE = 1024; // BUFSIZ; todo: check this - mjf

	// }================================================================== 




//        
//		 ** {==================================================================
//		@@ LUA_NUMBER is the type of numbers in Lua.
//		 ** CHANGE the following definitions only if you want to build Lua
//		 ** with a number type different from double. You may also need to
//		 ** change lua_number2int & lua_number2integer.
//		 ** ===================================================================
//		 

	///#define LUA_NUMBER_DOUBLE
	///#define LUA_NUMBER	double	/* declared in dotnet build with using statement */

//        
//		@@ LUAI_UACNUMBER is the result of an 'usual argument conversion'
//		@* over a number.
//		 
	///#define LUAI_UACNUMBER	double /* declared in dotnet build with using statement */


//        
//		@@ LUA_NUMBER_SCAN is the format for reading numbers.
//		@@ LUA_NUMBER_FMT is the format for writing numbers.
//		@@ lua_number2str converts a number to a string.
//		@@ LUAI_MAXNUMBER2STR is maximum size of previous conversion.
//		@@ lua_str2number converts a string to a number.
//		 
	public static final String LUA_NUMBER_SCAN = "%lf";
	public static final String LUA_NUMBER_FMT = "%.14g";

	public static CLib.CharPtr lua_number2str(double n) {
		if (n == (long)n) {
			return CLib.CharPtr.toCharPtr(Long.toString((long)n)); //FIXME:
		} else {
			return CLib.CharPtr.toCharPtr(Double.toString(n)); //FIXME:
		}
	}

	public static final int LUAI_MAXNUMBER2STR = 32; // 16 digits, sign, point, and \0 

	private static final String number_chars = "0123456789+-eE.";

	public static double lua_str2number(CLib.CharPtr s, CLib.CharPtr[] end) { //out
		end[0] = new CLib.CharPtr(s.chars, s.index);
		String str = "";
		while (end[0].get(0) == ' ') {
			end[0] = end[0].next();
		}
		while (number_chars.indexOf(end[0].get(0)) >= 0) {
			str += end[0].get(0);
			end[0] = end[0].next();
		}

		boolean[] isSuccess = new boolean[1];
		double result = ClassType.ConvertToDouble(str.toString(), isSuccess);
		if (isSuccess[0] == false) {
			end[0] = new CLib.CharPtr(s.chars, s.index);
		}
		return result;
	}

//        
//		@@ The luai_num* macros define the primitive operations over numbers.
//		 
	///#if LUA_CORE
	///#include <math.h>

    public static interface op_delegate {
        /*lua_Number*/
        /*lua_Number*/
        /*lua_Number*/
		double exec(double a, double b);
    }	
	
	public static double luai_numadd(double a, double b) { //lua_Number - lua_Number - lua_Number
		return ((a) + (b));
	}

	public static class luai_numadd_delegate implements op_delegate {
		public final double exec(double a, double b) { //lua_Number - lua_Number - lua_Number
			return luai_numadd(a, b);
		}
	}

	public static double luai_numsub(double a, double b) { //lua_Number - lua_Number - lua_Number
		return ((a) - (b));
	}

	public static class luai_numsub_delegate implements op_delegate {
		public final double exec(double a, double b) { //lua_Number - lua_Number - lua_Number
			return luai_numsub(a, b);
		}
	}

	public static double luai_nummul(double a, double b) { //lua_Number - lua_Number - lua_Number
		return ((a) * (b));
	}

	public static class luai_nummul_delegate implements op_delegate {
		public final double exec(double a, double b) { //lua_Number - lua_Number - lua_Number
			return luai_nummul(a, b);
		}
	}

	public static double luai_numdiv(double a, double b) { //lua_Number - lua_Number - lua_Number
		return ((a) / (b));
	}

	public static class luai_numdiv_delegate implements op_delegate {
		public final double exec(double a, double b) { //lua_Number - lua_Number - lua_Number
			return luai_numdiv(a, b);
		}
	}

	public static double luai_nummod(double a, double b) { //lua_Number - lua_Number - lua_Number
		return ((a) - Math.floor((a) / (b)) * (b));
	}

	public static class luai_nummod_delegate implements op_delegate {
		public final double exec(double a, double b) { //lua_Number - lua_Number - lua_Number
			return luai_nummod(a, b);
		}
	}

	public static double luai_numpow(double a, double b) { //lua_Number - lua_Number - lua_Number
		return (Math.pow(a, b));
	}

	public static class luai_numpow_delegate implements op_delegate {
		public final double exec(double a, double b) { //lua_Number - lua_Number - lua_Number
			return luai_numpow(a, b);
		}
	}

	public static double luai_numunm(double a) { //lua_Number - lua_Number
		return (-(a));
	}

	public static boolean luai_numeq(double a, double b) { //lua_Number - lua_Number
		return ((a) == (b));
	}

	public static boolean luai_numlt(double a, double b) { //lua_Number - lua_Number
		return ((a) < (b));
	}

	public static boolean luai_numle(double a, double b) { //lua_Number - lua_Number
		return ((a) <= (b));
	}

	public static boolean luai_numisnan(double a) { //lua_Number
		return ClassType.isNaN(a); //lua_Number
	}

	///#endif


//        
//		@@ lua_number2int is a macro to convert lua_Number to int.
//		@@ lua_number2integer is a macro to convert lua_Number to lua_Integer.
//		 ** CHANGE them if you know a faster way to convert a lua_Number to
//		 ** int (with any rounding method and without throwing errors) in your
//		 ** system. In Pentium machines, a naive typecast from double to int
//		 ** in C is extremely slow, so any alternative is worth trying.
//		 

	// On a Pentium, resort to a trick 
	///#if defined(LUA_NUMBER_DOUBLE) && !defined(LUA_ANSI) && !defined(__SSE2__) && \
	//	(defined(__i386) || defined (_M_IX86) || defined(__i386__))

	// On a Microsoft compiler, use assembler 
	///#if defined(_MSC_VER)

	///#define lua_number2int(i,d)   __asm fld d   __asm fistp i
	///#define lua_number2integer(i,n)		lua_number2int(i, n)

//         the next trick should work on any Pentium, but sometimes clashes
//		   with a DirectX idiosyncrasy 
	///#else

	//union luai_Cast { double l_d; long l_l; };
	///#define lua_number2int(i,d) \
	//  { volatile union luai_Cast u; u.l_d = (d) + 6755399441055744.0; (i) = u.l_l; }
	///#define lua_number2integer(i,n)		lua_number2int(i, n)

	///#endif


	// this option always works, but may be slow 
	///#else
	///#define lua_number2int(i,d)	((i)=(int)(d))
	///#define lua_number2integer(i,d)	((i)=(lua_Integer)(d))

	///#endif

	//private
	public static void lua_number2int(int[] i, double d) { //lua_Number - out
		i[0] = (int)d;
	}

	//private
	public static void lua_number2integer(int[] i, double n) { //lua_Number - out
		i[0] = (int)n;
	}

	// }================================================================== 


//        
//		@@ LUAI_USER_ALIGNMENT_T is a type that requires maximum alignment.
//		 ** CHANGE it if your system requires alignments larger than double. (For
//		 ** instance, if your system supports long doubles and they must be
//		 ** aligned in 16-byte boundaries, then you should add long double in the
//		 ** union.) Probably you do not need to change this.
//		 
	///#define LUAI_USER_ALIGNMENT_T	union { double u; void *s; long l; }

	public static class LuaException extends RuntimeException {
		public LuaState.lua_State L;
		public LuaDo.lua_longjmp c;
	
		public LuaException(LuaState.lua_State L, LuaDo.lua_longjmp c) 
		{ 
			this.L = L; 
			this.c = c;
		}
	}

//        
//		@@ LUAI_THROW/LUAI_TRY define how Lua does exception handling.
//		 ** CHANGE them if you prefer to use longjmp/setjmp even with C++
//		 ** or if want/don't to use _longjmp/_setjmp instead of regular
//		 ** longjmp/setjmp. By default, Lua handles errors with exceptions when
//		 ** compiling as C++ code, with _longjmp/_setjmp when asked to use them,
//		 ** and with longjmp/setjmp otherwise.
//		 
	///#if defined(__cplusplus)
	// C++ exceptions
	public static void LUAI_THROW(LuaState.lua_State L, LuaDo.lua_longjmp c) {
		throw new LuaException(L, c);
	}

	///#define LUAI_TRY(L,c,a)	try { a } catch(...) \
	//    { if ((c).status == 0) (c).status = -1; }
	public static void LUAI_TRY(LuaState.lua_State L, LuaDo.lua_longjmp c, Object a) {
		if (c.status == 0) {
			c.status = -1;
		}
	}
	///#define luai_jmpbuf	int  /* dummy variable */

	///#elif defined(LUA_USE_ULONGJMP)
	// in Unix, try _longjmp/_setjmp (more efficient) 
	///#define LUAI_THROW(L,c)	_longjmp((c).b, 1)
	///#define LUAI_TRY(L,c,a)	if (_setjmp((c).b) == 0) { a }
	///#define luai_jmpbuf	jmp_buf

	///#else
	// default handling with long jumps
	//public static void LUAI_THROW(lua_State L, lua_longjmp c) { c.b(1); }
	///#define LUAI_TRY(L,c,a)	if (setjmp((c).b) == 0) { a }
	///#define luai_jmpbuf	jmp_buf

	///#endif


//        
//		@@ LUA_MAXCAPTURES is the maximum number of captures that a pattern
//		@* can do during pattern-matching.
//		 ** CHANGE it if you need more captures. This limit is arbitrary.
//		 
	public static final int LUA_MAXCAPTURES = 32;

//        
//		@@ lua_tmpnam is the function that the OS library uses to create a
//		@* temporary name.
//		@@ LUA_TMPNAMBUFSIZE is the maximum size of a name created by lua_tmpnam.
//		 ** CHANGE them if you have an alternative to tmpnam (which is considered
//		 ** insecure) or if you want the original tmpnam anyway.  By default, Lua
//		 ** uses tmpnam except when POSIX is available, where it uses mkstemp.
//		 
	///#if loslib_c || luaall_c
	///#if LUA_USE_MKSTEMP
//		//#include <unistd.h>
//		public const int LUA_TMPNAMBUFSIZE = 32;
//		//#define lua_tmpnam(b,e)	{ \
//		//    strcpy(b, "/tmp/lua_XXXXXX"); \
//		//    e = mkstemp(b); \
//		//    if (e != -1) close(e); \
//		//    e = (e == -1); }
//
//		//#else
//		public const int LUA_TMPNAMBUFSIZE	= L_tmpnam;
//		
//		public static void lua_tmpnam(CharPtr b, int e)
//		{ 
//			e = (tmpnam(b) == null) ? 1 : 0; 
//		}
	///#endif

	///#endif


//        
//		@@ lua_popen spawns a new process connected to the current one through
//		@* the file streams.
//		 ** CHANGE it if you have a way to implement it in your system.
//		 
	///#if LUA_USE_POPEN

	///#define lua_popen(L,c,m)	((void)L, fflush(null), popen(c,m))
	///#define lua_pclose(L,file)	((void)L, (pclose(file) != -1))

	///#elif LUA_WIN

	///#define lua_popen(L,c,m)	((void)L, _popen(c,m))
	///#define lua_pclose(L,file)	((void)L, (_pclose(file) != -1))

	///#else

	public static StreamProxy lua_popen(LuaState.lua_State L, CLib.CharPtr c, CLib.CharPtr m) {
		LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr(LUA_QL("popen") + " not supported"));
		return null;
	}

	public static int lua_pclose(LuaState.lua_State L, StreamProxy file) {
		return 0;
	}

	///#endif

//        
//		@@ LUA_DL_* define which dynamic-library system Lua should use.
//		 ** CHANGE here if Lua has problems choosing the appropriate
//		 ** dynamic-library system for your platform (either Windows' DLL, Mac's
//		 ** dyld, or Unix's dlopen). If your system is some kind of Unix, there
//		 ** is a good chance that it has dlopen, so LUA_DL_DLOPEN will work for
//		 ** it.  To use dlopen you also need to adapt the src/Makefile (probably
//		 ** adding -ldl to the linker options), so Lua does not select it
//		 ** automatically.  (When you change the makefile to add -ldl, you must
//		 ** also add -DLUA_USE_DLOPEN.)
//		 ** If you do not want any kind of dynamic library, undefine all these
//		 ** options.
//		 ** By default, _WIN32 gets LUA_DL_DLL and MAC OS X gets LUA_DL_DYLD.
//		 
	///#if LUA_USE_DLOPEN
	///#define LUA_DL_DLOPEN
	///#endif

	///#if LUA_WIN
	///#define LUA_DL_DLL
	///#endif


//        
//		@@ LUAI_EXTRASPACE allows you to add user-specific data in a lua_State
//		@* (the data goes just *before* the lua_State pointer).
//		 ** CHANGE (define) this if you really need that. This value must be
//		 ** a multiple of the maximum alignment required for your machine.
//		 
	public static final int LUAI_EXTRASPACE = 0;

//        
//		@@ luai_userstate* allow user-specific actions on threads.
//		 ** CHANGE them if you defined LUAI_EXTRASPACE and need to do something
//		 ** extra when a thread is created/deleted/resumed/yielded.
//		 
	public static void luai_userstateopen(LuaState.lua_State L) {

	}
	public static void luai_userstateclose(LuaState.lua_State L) {

	}
	public static void luai_userstatethread(LuaState.lua_State L, LuaState.lua_State L1) {

	}
	public static void luai_userstatefree(LuaState.lua_State L) {

	}
	public static void luai_userstateresume(LuaState.lua_State L, int n) {

	}
	public static void luai_userstateyield(LuaState.lua_State L, int n) {

	}

//        
//		@@ LUA_INTFRMLEN is the length modifier for integer conversions
//		@* in 'string.format'.
//		@@ LUA_INTFRM_T is the integer type correspoding to the previous length
//		@* modifier.
//		 ** CHANGE them if your system supports long long or does not support long.
//		 

	///#if LUA_USELONGLONG
//		public const string LUA_INTFRMLEN = "ll";
//		//#define LUA_INTFRM_T		long long

	///#else

	public static final String LUA_INTFRMLEN = "l";
	///#define LUA_INTFRM_T		long			/* declared in dotnet build with using statement */

	///#endif



	// =================================================================== 

//        
//		 ** Local configuration. You can use this space to add your redefinitions
//		 ** without modifying the main part of the file.
//		 

}
//public delegate Double/*lua_Number*/ op_delegate(Double/*lua_Number*/ a, Double/*lua_Number*/ b);
//public interface op_delegate
//{
//	Double/*lua_Number*/ exec(Double/*lua_Number*/ a, Double/*lua_Number*/ b);
//}	
