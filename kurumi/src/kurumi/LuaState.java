package kurumi;

//
// ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
// ** Global State
// ** See Copyright Notice in lua.h
// 

//using lu_byte = System.Byte;
//using lu_int32 = System.Int32;
//using lu_mem = System.UInt32;
//using TValue = Lua.TValue;
//using StkId = TValue;
//using ptrdiff_t = System.Int32;
//using Instruction = System.UInt32;

public class LuaState {
	// table of globals 
	public static TValue gt(lua_State L) {
		return L.l_gt;
	}

	// registry 
	public static TValue registry(lua_State L) {
		return G(L).l_registry;
	}

	// extra stack space to handle TM calls and some other extras 
	public static final int EXTRA_STACK = 5;

	public static final int BASIC_CI_SIZE = 8;

	public static final int BASIC_STACK_SIZE = (2*Lua.LUA_MINSTACK);

    /*
    ** informations about a call
    */
	public static class CallInfo implements LuaObject.ArrayElement {
        private CallInfo[] values = null;
        private int index = -1;
    
        public TValue base_;  /*StkId*/ /* base for this function */
        public TValue func;  /*StkId*/ /* function index in the stack */
        public TValue top;  /*StkId*/ /* top for this function */
        public LuaCode.InstructionPtr savedpc;
        public int nresults;  /* expected number of results from this function */
        public int tailcalls;  /* number of tail calls lost under this entry */
        
        public void set_index(int index)
        {
            this.index = index;
        }
    
		public void set_array(Object array)
        {
            this.values = (CallInfo[])array;
            ClassType.Assert(this.values != null);
        }
        
        public CallInfo get(int offset)
        {
        	return values[index + offset]; 
        }
    
        public static CallInfo plus(CallInfo value, int offset)
        {
            return value.values[value.index + offset];
        }
    
        public static CallInfo minus(CallInfo value, int offset)
        {
            return value.values[value.index - offset];
        }
    
        public static int minus(CallInfo ci, CallInfo[] values)
        {
            ClassType.Assert(ci.values == values);
            return ci.index;
        }
    
        public static int minus(CallInfo ci1, CallInfo ci2)
        {
            ClassType.Assert(ci1.values == ci2.values);
            return ci1.index - ci2.index;
        }
    
		public static boolean lessThan(CallInfo ci1, CallInfo ci2) 
        {
            ClassType.Assert(ci1.values == ci2.values);
            return ci1.index < ci2.index;
        }
    
		public static boolean lessEqual(CallInfo ci1, CallInfo ci2) 
        {
            ClassType.Assert(ci1.values == ci2.values);
            return ci1.index <= ci2.index;
        }
    
		public static boolean greaterThan(CallInfo ci1, CallInfo ci2) 
        {
            ClassType.Assert(ci1.values == ci2.values);
            return ci1.index > ci2.index;
        }
    
		public static boolean greaterEqual(CallInfo ci1, CallInfo ci2) 
        {
            ClassType.Assert(ci1.values == ci2.values);
            return ci1.index >= ci2.index;
        }
    
        public static CallInfo inc(/*ref*/ CallInfo[] value)
        {
        	value[0] = value[0].get(1);
        	return value[0].get(-1);
        }

        public static CallInfo dec(/*ref*/ CallInfo[] value)
        {
        	value[0] = value[0].get(-1);
        	return value[0].get(1);
        }
    }
	
	public static LuaObject.Closure curr_func(lua_State L) {
		return (LuaObject.clvalue(L.ci.func));
	}

	public static LuaObject.Closure ci_func(CallInfo ci) {
		return (LuaObject.clvalue(ci.func));
	}

	public static boolean f_isLua(CallInfo ci) {
		return ci_func(ci).c.getIsC() == 0;
	}

	public static boolean isLua(CallInfo ci) {
		return (LuaObject.ttisfunction((ci).func) && f_isLua(ci));
	}

	/*
	 ** `global state', shared by all threads of this state
	 */
	public static class global_State {
		public stringtable strt = new stringtable(); /* hash table for strings */
		public Lua.lua_Alloc frealloc;  /* function to reallocate memory */
		public Object ud;         /* auxiliary data to `frealloc' */
		public byte currentwhite;  /*Byte*/ /*lu_byte*/
		public byte gcstate; /*Byte*/ /*lu_byte*/  /* state of garbage collector */
		public int sweepstrgc;  /* position of sweep in `strt' */
		public LuaState.GCObject rootgc;  /* list of all collectable objects */
		public LuaState.GCObjectRef sweepgc;  /* position of sweep in `rootgc' */
		public LuaState.GCObject gray;  /* list of gray objects */
		public LuaState.GCObject grayagain;  /* list of objects to be traversed atomically */
		public LuaState.GCObject weak;  /* list of weak tables (to be cleared) */
		public LuaState.GCObject tmudata;  /* last element of list of userdata to be GC */
		public Mbuffer buff = new Mbuffer();  /* temporary buffer for string concatentation */
		public long/*UInt32*//*lu_mem*/ GCthreshold;
		public long/*UInt32*//*lu_mem*/ totalbytes;  /* number of bytes currently allocated */
		public long/*UInt32*//*lu_mem*/ estimate;  /* an estimate of number of bytes actually in use */
		public long/*UInt32*//*lu_mem*/ gcdept;  /* how much GC is `behind schedule' */
		public int gcpause;  /* size of pause between successive GCs */
		public int gcstepmul;  /* GC `granularity' */
		public Lua.lua_CFunction panic;  /* to be called in unprotected errors */
		public TValue l_registry = new TValue();
		public lua_State mainthread;
		public UpVal uvhead = new UpVal();  /* head of double-linked list of all open upvalues */
		public Table[] mt = new Table[LuaObject.NUM_TAGS];  /* metatables for basic types */
		public TString[] tmname = new TString[TMS.TM_N.getValue()]; // array with tag-method names 
	}	
	
	/*
	 ** `per thread' state
	 */
	public static class lua_State extends LuaState.GCObject 
	{
		public byte status; /*Byte*/ /*lu_byte*/
		public TValue/*StkId*/ top;  /* first free slot in the stack */
		public TValue/*StkId*/ base_;  /* base of current function */
		public LuaState.global_State l_G;
		public LuaState.CallInfo ci;  /* call info for current function */
		public LuaCode.InstructionPtr savedpc = new LuaCode.InstructionPtr();  /* `savedpc' of current function */
		public TValue/*StkId*/ stack_last;  /* last free slot in the stack */
		public TValue[]/*StkId[]*/ stack;  /* stack base */
		public LuaState.CallInfo end_ci;  /* points after end of ci array*/
		public LuaState.CallInfo[] base_ci;  /* array of CallInfo's */
		public int stacksize;
		public int size_ci;  /* size of array `base_ci' */
		public int/*ushort*/ nCcalls;  /* number of nested C calls */
		public int/*ushort*/ baseCcalls;  /* nested C calls when resuming coroutine */
		public byte hookmask; /*Byte*/ /*lu_byte*/
		public byte allowhook; /*Byte*/ /*lu_byte*/
		public int basehookcount;
		public int hookcount;
		public Lua.lua_Hook hook;
		public TValue l_gt = new TValue();  /* table of globals */
		public TValue env = new TValue();  /* temporary place for environments */
		public LuaState.GCObject openupval;  /* list of open upvalues in this stack */
		public LuaState.GCObject gclist;
		public LuaDo.lua_longjmp errorJmp;  /* current error recover point */
		public int/*Int32*//*ptrdiff_t*/ errfunc;  /* current error handling function (stack index) */
	}
	
	public static global_State G(lua_State L) {
		return L.l_G;
	}

	public static void G_set(lua_State L, global_State s) {
		L.l_G = s;
	}
	
	/*
	 ** Union of all collectable objects (not a union anymore in the C# port)
	 */
	public static class GCObject extends LuaObject.GCheader implements LuaObject.ArrayElement 
	{
		// todo: remove this?
		//private GCObject[] values = null;
		//private int index = -1;
		
		public void set_index(int index)
		{
			//this.index = index;
		}
		
		public void set_array(Object array) 
		{
			//this.values = (GCObject[])array;
			//ClassType.Assert(this.values != null);
		}

        public LuaObject.GCheader getGch()
        {
            return (LuaObject.GCheader)this;
        }

        public TString getTs()
        {
            return (TString)this;
        }

        public Udata getU()
        {
            return (Udata)this;
        }

        public LuaObject.Closure getCl()
        {
            return (LuaObject.Closure)this;
        }

        public Table getH()
        {
            return (Table)this;
        }

        public Proto getP()
        {
            return (Proto)this;
        }

        public UpVal getUv()
        {
            return (UpVal)this;
        }

        public lua_State getTh()
        {
            return (lua_State)this;
        }
	}
	
	/*
	 ** this interface and is used for implementing GCObject references,
	 ** it's used to emulate the behaviour of a C-style GCObject 
	 */
	public static interface GCObjectRef {
		void set(LuaState.GCObject value);
		LuaState.GCObject get();
	}

	public static class ArrayRef implements GCObjectRef, LuaObject.ArrayElement {
		// ArrayRef is used to reference GCObject objects in an array, the next two members
		// point to that array and the index of the GCObject element we are referencing
		private GCObject[] array_elements;
		private int array_index;

		// ArrayRef is itself stored in an array and derived from ArrayElement, the next
		// two members refer to itself i.e. the array and index of it's own instance.
		private ArrayRef[] vals;
		private int index;

		public ArrayRef() 
		{
			this.array_elements = null;
			this.array_index = 0;
			this.vals = null;
			this.index = 0;
		}

		public ArrayRef(GCObject[] array_elements, int array_index) 
		{
			this.array_elements = array_elements;
			this.array_index = array_index;
			this.vals = null;
			this.index = 0;
		}

		public void set(GCObject value) 
		{
			array_elements[array_index] = value;
		}

		public GCObject get() 
		{
			return array_elements[array_index];
		}

		public void set_index(int index) 
		{
			this.index = index;
		}

		public void set_array(Object vals) 
		{
			// don't actually need this
			this.vals = (ArrayRef[])vals;
			ClassType.Assert(this.vals != null);
		}
	}	
	
	// macros to convert a GCObject into a specific value 
	public static TString rawgco2ts(GCObject o) {
		return (TString)LuaLimits.check_exp(o.getGch().tt == Lua.LUA_TSTRING, o.getTs());
	}

	public static TString gco2ts(GCObject o) {
		return (TString)(rawgco2ts(o).getTsv());
	}

	public static Udata rawgco2u(GCObject o) {
		return (Udata)LuaLimits.check_exp(o.getGch().tt == Lua.LUA_TUSERDATA, o.getU());
	}

	public static Udata gco2u(GCObject o) {
		return (Udata)(rawgco2u(o).uv);
	}

	public static LuaObject.Closure gco2cl(GCObject o) {
		return (LuaObject.Closure)LuaLimits.check_exp(o.getGch().tt == Lua.LUA_TFUNCTION, o.getCl());
	}

	public static Table gco2h(GCObject o) {
		return (Table)LuaLimits.check_exp(o.getGch().tt == Lua.LUA_TTABLE, o.getH());
	}

	public static Proto gco2p(GCObject o) {
		return (Proto)LuaLimits.check_exp(o.getGch().tt == LuaObject.LUA_TPROTO, o.getP());
	}

	public static UpVal gco2uv(GCObject o) {
		return (UpVal)LuaLimits.check_exp(o.getGch().tt == LuaObject.LUA_TUPVAL, o.getUv());
	}

	public static UpVal ngcotouv(GCObject o) {
		return (UpVal)LuaLimits.check_exp((o == null) || (o.getGch().tt == LuaObject.LUA_TUPVAL), o.getUv());
	}

	public static lua_State gco2th(GCObject o) {
		return (lua_State)LuaLimits.check_exp(o.getGch().tt == Lua.LUA_TTHREAD, o.getTh());
	}

	// macro to convert any Lua object into a GCObject 
	public static GCObject obj2gco(Object v) {
		return (GCObject)v;
	}

	public static int state_size(Object x, ClassType t) {
		return t.GetMarshalSizeOf() + LuaConf.LUAI_EXTRASPACE; //Marshal.SizeOf(x)
	}

//        
//		public static lu_byte fromstate(object l)
//		{
//			return (lu_byte)(l - LUAI_EXTRASPACE);
//		}
//		

	public static lua_State tostate(Object l) {
		ClassType.Assert(LuaConf.LUAI_EXTRASPACE == 0, "LUAI_EXTRASPACE not supported");
		return (lua_State)l;
	}

	/*
	 ** Main thread combines a thread state and the global state
	 */
	public static class LG extends lua_State {
		public LuaState.global_State g = new LuaState.global_State();
		
		public lua_State getL() 
		{
		    return this; 
		}
	}
	
	private static void stack_init(lua_State L1, lua_State L) {
		// initialize CallInfo array 
		L1.base_ci = LuaMem.luaM_newvector_CallInfo(L, BASIC_CI_SIZE, new ClassType(ClassType.TYPE_CALLINFO));
		L1.ci = L1.base_ci[0];
		L1.size_ci = BASIC_CI_SIZE;
		L1.end_ci = L1.base_ci[L1.size_ci - 1];
		// initialize stack array 
		L1.stack = LuaMem.luaM_newvector_TValue(L, BASIC_STACK_SIZE + EXTRA_STACK, new ClassType(ClassType.TYPE_TVALUE));
		L1.stacksize = BASIC_STACK_SIZE + EXTRA_STACK;
		L1.top = L1.stack[0];
		L1.stack_last = L1.stack[L1.stacksize - EXTRA_STACK - 1];
		// initialize first ci 
		L1.ci.func = L1.top;
		TValue[] top = new TValue[1];
		top[0] = L1.top;
		TValue ret = TValue.inc(top); //ref - StkId
		L1.top = top[0];
		LuaObject.setnilvalue(ret); // `function' entry for this `ci' 
		L1.base_ = L1.ci.base_ = L1.top;
		L1.ci.top = TValue.plus(L1.top, Lua.LUA_MINSTACK);
	}

	private static void freestack(lua_State L, lua_State L1) {
		LuaMem.luaM_freearray_CallInfo(L, L1.base_ci, new ClassType(ClassType.TYPE_CALLINFO));
		LuaMem.luaM_freearray_TValue(L, L1.stack, new ClassType(ClassType.TYPE_TVALUE));
	}

//        
//		 ** open parts that may cause memory-allocation errors
//		 
	private static void f_luaopen(lua_State L, Object ud) {
		global_State g = G(L);
		//UNUSED(ud);
		stack_init(L, L); // init stack 
		LuaObject.sethvalue(L, gt(L), LuaTable.luaH_new(L, 0, 2)); // table of globals 
		LuaObject.sethvalue(L, registry(L), LuaTable.luaH_new(L, 0, 2)); // registry 
		LuaString.luaS_resize(L, LuaLimits.MINSTRTABSIZE); // initial size of string table 
		LuaTM.luaT_init(L);
		LuaLex.luaX_init(L);
		LuaString.luaS_fix(LuaString.luaS_newliteral(L, LuaConf.CharPtr.toCharPtr(LuaMem.MEMERRMSG)));
		g.GCthreshold = 4 * g.totalbytes;
	}

	public static class f_luaopen_delegate implements Pfunc {
		public final void exec(lua_State L, Object ud) {
			f_luaopen(L, ud);
		}
	}

	private static void preinit_state(lua_State L, global_State g) {
		G_set(L, g);
		L.stack = null;
		L.stacksize = 0;
		L.errorJmp = null;
		L.hook = null;
		L.hookmask = 0;
		L.basehookcount = 0;
		L.allowhook = 1;
		LuaDebug.resethookcount(L);
		L.openupval = null;
		L.size_ci = 0;
		L.nCcalls = L.baseCcalls = 0;
		L.status = 0;
		L.base_ci = null;
		L.ci = null;
		L.savedpc = new LuaCode.InstructionPtr();
		L.errfunc = 0;
		LuaObject.setnilvalue(gt(L));
	}


	private static void close_state(lua_State L) {
		global_State g = G(L);
		LuaFunc.luaF_close(L, L.stack[0]); // close all upvalues for this thread 
		LuaGC.luaC_freeall(L); // collect all objects 
		LuaLimits.lua_assert(g.rootgc == obj2gco(L));
		LuaLimits.lua_assert(g.strt.nuse == 0);
		LuaMem.luaM_freearray_GCObject(L, G(L).strt.hash, new ClassType(ClassType.TYPE_GCOBJECT));
		LuaZIO.luaZ_freebuffer(L, g.buff);
		freestack(L, L);
		LuaLimits.lua_assert(g.totalbytes == LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_LG))); //typeof(LG)
		//g.frealloc(g.ud, fromstate(L), (uint)state_size(typeof(LG)), 0);
	}

	//private
	public static lua_State luaE_newthread(lua_State L) {
		//lua_State L1 = tostate(luaM_malloc(L, state_size(typeof(lua_State))));
		lua_State L1 = LuaMem.luaM_new_lua_State(L, new ClassType(ClassType.TYPE_LUA_STATE));
		LuaGC.luaC_link(L, obj2gco(L1), (byte)Lua.LUA_TTHREAD);
		preinit_state(L1, G(L));
		stack_init(L1, L); // init stack 
		LuaObject.setobj2n(L, gt(L1), gt(L)); // share table of globals 
		L1.hookmask = L.hookmask;
		L1.basehookcount = L.basehookcount;
		L1.hook = L.hook;
		LuaDebug.resethookcount(L1);
		LuaLimits.lua_assert(LuaGC.iswhite(obj2gco(L1)));
		return L1;
	}

	//private
	public static void luaE_freethread(lua_State L, lua_State L1) {
		LuaFunc.luaF_close(L1, L1.stack[0]); // close all upvalues for this thread 
		LuaLimits.lua_assert(L1.openupval == null);
		LuaConf.luai_userstatefree(L1);
		freestack(L, L1);
		//luaM_freemem(L, fromstate(L1));
	}

	public static lua_State lua_newstate(Lua.lua_Alloc f, Object ud) {
		int i;
		lua_State L;
		global_State g;
		//object l = f(ud, null, 0, (uint)state_size(typeof(LG)));
		Object l = f.exec(new ClassType(ClassType.TYPE_LG)); //typeof(LG)
		if (l == null) {
			return null;
		}
		L = tostate(l);
		g = ((LG)((L instanceof LG) ? L : null)).g;
		L.next = null;
		L.tt = Lua.LUA_TTHREAD;
		g.currentwhite = (byte)LuaGC.bit2mask(LuaGC.WHITE0BIT, LuaGC.FIXEDBIT); //lu_byte
		L.marked = LuaGC.luaC_white(g);
		byte marked = L.marked; // can't pass properties in as ref - lu_byte
		byte[] marked_ref = new byte[1];
		marked_ref[0] = marked;
		LuaGC.set2bits(marked_ref, LuaGC.FIXEDBIT, LuaGC.SFIXEDBIT); //ref
		marked = marked_ref[0];
		L.marked = marked;
		preinit_state(L, g);
		g.frealloc = f;
		g.ud = ud;
		g.mainthread = L;
		g.uvhead.u.l.prev = g.uvhead;
		g.uvhead.u.l.next = g.uvhead;
		g.GCthreshold = 0; // mark it as unfinished state 
		g.strt.size = 0;
		g.strt.nuse = 0;
		g.strt.hash = null;
		LuaObject.setnilvalue(registry(L));
		LuaZIO.luaZ_initbuffer(L, g.buff);
		g.panic = null;
		g.gcstate = LuaGC.GCSpause;
		g.rootgc = obj2gco(L);
		g.sweepstrgc = 0;
		g.sweepgc = new RootGCRef(g);
		g.gray = null;
		g.grayagain = null;
		g.weak = null;
		g.tmudata = null;
		g.totalbytes = (long)LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_LG)); //typeof(LG) - uint
		g.gcpause = LuaConf.LUAI_GCPAUSE;
		g.gcstepmul = LuaConf.LUAI_GCMUL;
		g.gcdept = 0;
		for (i = 0; i < LuaObject.NUM_TAGS; i++) {
			g.mt[i] = null;
		}
		if (LuaDo.luaD_rawrunprotected(L, new f_luaopen_delegate(), null) != 0) {
			// memory allocation error: free partial state 
			close_state(L);
			L = null;
		}
		else {
			LuaConf.luai_userstateopen(L);
		}
		return L;
	}

	private static void callallgcTM(lua_State L, Object ud) {
		//UNUSED(ud);
		LuaGC.luaC_callGCTM(L); // call GC metamethods for all udata 
	}

	public static class callallgcTM_delegate implements Pfunc {
		public final void exec(lua_State L, Object ud) {
			callallgcTM(L, ud);
		}
	}

	public static void lua_close(lua_State L) {
		L = G(L).mainthread; // only the main thread can be closed 
		LuaLimits.lua_lock(L);
		LuaFunc.luaF_close(L, L.stack[0]); // close all upvalues for this thread 
		LuaGC.luaC_separateudata(L, 1); // separate udata that have GC metamethods 
		L.errfunc = 0; // no error function during GC metamethods 
		do {
			// repeat until no more errors 
			L.ci = L.base_ci[0];
			L.base_ = L.top = L.ci.base_;
			L.nCcalls = L.baseCcalls = 0;
		} while (LuaDo.luaD_rawrunprotected(L, new callallgcTM_delegate(), null) != 0);
		LuaLimits.lua_assert(G(L).tmudata == null);
		LuaConf.luai_userstateclose(L);
		close_state(L);
	}
}