package kurumi;

//
// ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
// ** Some generic functions over Lua objects
// ** See Copyright Notice in lua.h
// 

//using TValue = Lua.TValue;
//using StkId = TValue;
//using lu_byte = System.Byte;
//using lua_Number = System.Double;
//using l_uacNumber = System.Double;
//using Instruction = System.UInt32;

public class LuaObject {
	// tags for values visible from Lua 
	public static final int LAST_TAG = Lua.LUA_TTHREAD;

	public static final int NUM_TAGS = (LAST_TAG + 1);

//        
//		 ** Extra tags for non-values
//		 
	public static final int LUA_TPROTO = (LAST_TAG + 1);
	public static final int LUA_TUPVAL = (LAST_TAG + 2);
	public static final int LUA_TDEADKEY = (LAST_TAG + 3);

	public interface ArrayElement 
	{
		void set_index(int index);
		void set_array(Object array);
	}	
	
//        
//		 ** Tagged Values
//		 

	///#define TValuefields	Value value; int tt

	// Macros to test type 
	public static boolean ttisnil(TValue o) {
		return (ttype(o) == Lua.LUA_TNIL);
	}

	public static boolean ttisnumber(TValue o) {
		return (ttype(o) == Lua.LUA_TNUMBER);
	}

	public static boolean ttisstring(TValue o) {
		return (ttype(o) == Lua.LUA_TSTRING);
	}

	public static boolean ttistable(TValue o) {
		return (ttype(o) == Lua.LUA_TTABLE);
	}

	public static boolean ttisfunction(TValue o) {
		return (ttype(o) == Lua.LUA_TFUNCTION);
	}

	public static boolean ttisboolean(TValue o) {
		return (ttype(o) == Lua.LUA_TBOOLEAN);
	}

	public static boolean ttisuserdata(TValue o) {
		return (ttype(o) == Lua.LUA_TUSERDATA);
	}

	public static boolean ttisthread(TValue o) {
		return (ttype(o) == Lua.LUA_TTHREAD);
	}

	public static boolean ttislightuserdata(TValue o) {
		return (ttype(o) == Lua.LUA_TLIGHTUSERDATA);
	}

	// Macros to access values 
	public static int ttype(TValue o) {
		return o.tt;
	}

	public static int ttype(CommonHeader o) {
		return o.tt;
	}

	public static GCObject gcvalue(TValue o) {
		return (GCObject)LuaLimits.check_exp(iscollectable(o), o.value.gc);
	}

	public static Object pvalue(TValue o) {
		return (Object)LuaLimits.check_exp(ttislightuserdata(o), o.value.p);
	}

	public static double nvalue(TValue o) { //lua_Number
		return ((Double)LuaLimits.check_exp(ttisnumber(o), o.value.n)).doubleValue(); //lua_Number
	}

	public static TString rawtsvalue(TValue o) {
		return (TString)LuaLimits.check_exp(ttisstring(o), o.value.gc.getTs());
	}

	public static TString_tsv tsvalue(TValue o) {
		return rawtsvalue(o).getTsv();
	}

	public static Udata rawuvalue(TValue o) {
		return (Udata)LuaLimits.check_exp(ttisuserdata(o), o.value.gc.getU());
	}

	public static Udata_uv uvalue(TValue o) {
		return rawuvalue(o).uv;
	}

	public static Closure clvalue(TValue o) {
		return (Closure)LuaLimits.check_exp(ttisfunction(o), o.value.gc.getCl());
	}

	public static Table hvalue(TValue o) {
		return (Table)LuaLimits.check_exp(ttistable(o), o.value.gc.getH());
	}

	public static int bvalue(TValue o) {
		return ((Integer)LuaLimits.check_exp(ttisboolean(o), o.value.b)).intValue();
	}

	public static lua_State thvalue(TValue o) {
		return (lua_State)LuaLimits.check_exp(ttisthread(o), o.value.gc.getTh());
	}

	public static int l_isfalse(TValue o) {
		return ((ttisnil(o) || (ttisboolean(o) && bvalue(o) == 0))) ? 1 : 0;
	}

//        
//		 ** for internal debug only
//		 
	public static void checkconsistency(TValue obj) {
		LuaLimits.lua_assert(!iscollectable(obj) || (ttype(obj) == (obj).value.gc.getGch().tt));
	}

	public static void checkliveness(global_State g, TValue obj) {
		LuaLimits.lua_assert(!iscollectable(obj) || ((ttype(obj) == obj.value.gc.getGch().tt) && !LuaGC.isdead(g, obj.value.gc)));
	}


	// Macros to set values 
	public static void setnilvalue(TValue obj) {
		obj.tt = Lua.LUA_TNIL;
	}

	public static void setnvalue(TValue obj, double x) { //lua_Number
		obj.value.n = x;
		obj.tt = Lua.LUA_TNUMBER;
	}

	public static void setpvalue(TValue obj, Object x) {
		obj.value.p = x;
		obj.tt = Lua.LUA_TLIGHTUSERDATA;
	}

	public static void setbvalue(TValue obj, int x) {
		obj.value.b = x;
		obj.tt = Lua.LUA_TBOOLEAN;
	}

	public static void setsvalue(lua_State L, TValue obj, GCObject x) {
		obj.value.gc = x;
		obj.tt = Lua.LUA_TSTRING;
		checkliveness(LuaState.G(L), obj);
	}

	public static void setuvalue(lua_State L, TValue obj, GCObject x) {
		obj.value.gc = x;
		obj.tt = Lua.LUA_TUSERDATA;
		checkliveness(LuaState.G(L), obj);
	}

	public static void setthvalue(lua_State L, TValue obj, GCObject x) {
		obj.value.gc = x;
		obj.tt = Lua.LUA_TTHREAD;
		checkliveness(LuaState.G(L), obj);
	}

	public static void setclvalue(lua_State L, TValue obj, Closure x) {
		obj.value.gc = x;
		obj.tt = Lua.LUA_TFUNCTION;
		checkliveness(LuaState.G(L), obj);
	}

	public static void sethvalue(lua_State L, TValue obj, Table x) {
		obj.value.gc = x;
		obj.tt = Lua.LUA_TTABLE;
		checkliveness(LuaState.G(L), obj);
	}

	public static void setptvalue(lua_State L, TValue obj, Proto x) {
		obj.value.gc = x;
		obj.tt = LUA_TPROTO;
		checkliveness(LuaState.G(L), obj);
	}

	public static void setobj(lua_State L, TValue obj1, TValue obj2) {
		obj1.value.copyFrom(obj2.value);
		obj1.tt = obj2.tt;
		checkliveness(LuaState.G(L), obj1);
	}


//        
//		 ** different types of sets, according to destination
//		 

	// from stack to (same) stack 
	///#define setobjs2s	setobj
	public static void setobjs2s(lua_State L, TValue obj, TValue x) {
		setobj(L, obj, x);
	}
	//to stack (not from same stack)

	///#define setobj2s	setobj
	public static void setobj2s(lua_State L, TValue obj, TValue x) {
		setobj(L, obj, x);
	}

	///#define setsvalue2s	setsvalue
	public static void setsvalue2s(lua_State L, TValue obj, TString x) {
		setsvalue(L, obj, x);
	}

	///#define sethvalue2s	sethvalue
	public static void sethvalue2s(lua_State L, TValue obj, Table x) {
		sethvalue(L, obj, x);
	}

	///#define setptvalue2s	setptvalue
	public static void setptvalue2s(lua_State L, TValue obj, Proto x) {
		setptvalue(L, obj, x);
	}

	// from table to same table 
	///#define setobjt2t	setobj
	public static void setobjt2t(lua_State L, TValue obj, TValue x) {
		setobj(L, obj, x);
	}

	// to table 
	///#define setobj2t	setobj
	public static void setobj2t(lua_State L, TValue obj, TValue x) {
		setobj(L, obj, x);
	}

	// to new object 
	///#define setobj2n	setobj
	public static void setobj2n(lua_State L, TValue obj, TValue x) {
		setobj(L, obj, x);
	}

	///#define setsvalue2n	setsvalue
	public static void setsvalue2n(lua_State L, TValue obj, TString x) {
		setsvalue(L, obj, x);
	}

	public static void setttype(TValue obj, int tt) {
		obj.tt = tt;
	}

	public static boolean iscollectable(TValue o) {
		return (ttype(o) >= Lua.LUA_TSTRING);
	}

	public static LuaConf.CharPtr getstr(TString ts) {
		return ts.str;
	}

	public static LuaConf.CharPtr svalue(TValue o) { //StkId
		return getstr(rawtsvalue(o));
	}

	// masks for new-style vararg 
	public static final int VARARG_HASARG = 1;
	public static final int VARARG_ISVARARG = 2;
	public static final int VARARG_NEEDSARG = 4;

	
	public static class CClosure extends ClosureType {
		public lua_CFunction f;
		public TValue[] upvalue;
	
		public CClosure(ClosureHeader header) 
		{
			super(header);
						
		}
	}	
	
	
	
	public static class Closure extends ClosureHeader {	
		public LuaObject.CClosure c;
		public LClosure l;
		
		public Closure()
		{
			c = new LuaObject.CClosure(this);
			l = new LClosure(this);
		}
	}
	
	public static boolean iscfunction(TValue o) {
		return ((ttype(o) == Lua.LUA_TFUNCTION) && (clvalue(o).c.getIsC() != 0));
	}

	public static boolean isLfunction(TValue o) {
		return ((ttype(o) == Lua.LUA_TFUNCTION) && (clvalue(o).c.getIsC() == 0));
	}

//        
//		 ** `module' operation for hashing (size is always a power of 2)
//		 
	///#define lmod(s,size) \
	//    (check_exp((size&(size-1))==0, (cast(int, (s) & ((size)-1)))))

	public static int twoto(int x) {
		return 1 << x;
	}

	public static int sizenode(Table t) {
		return twoto(t.lsizenode);
	}

	public static TValue luaO_nilobject_ = new TValue(new Value(), Lua.LUA_TNIL);
	public static TValue luaO_nilobject = luaO_nilobject_;

	public static int ceillog2(int x) {
		return luaO_log2((int)(x - 1)) + 1; //uint
	}

//        
//		 ** converts an integer to a "floating point byte", represented as
//		 ** (eeeeexxx), where the real value is (1xxx) * 2^(eeeee - 1) if
//		 ** eeeee != 0 and (xxx) otherwise.
//		 
	public static int luaO_int2fb(int x) { //uint
		int e = 0; // expoent 
		while (x >= 16) {
			x = (x+1) >> 1;
			e++;
		}
		if (x < 8) {
			return (int)x;
		}
		else {
			return ((e + 1) << 3) | (LuaLimits.cast_int(x) - 8);
		}
	}

	// converts back 
	public static int luaO_fb2int(int x) {
		int e = (x >> 3) & 31;
		if (e == 0) {
			return x;
		}
		else {
			return ((x & 7)+8) << (e - 1);
		}
	}

	private final static byte [] log_2 = { 0,1,2,2,3,3,3,3,4,4,4,4,4,4,4,4,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5, 6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6, 7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7, 7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7, 8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8, 8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8, 8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8, 8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8 };

	public static int luaO_log2(long x) { //uint
		int l = -1;
		while (x >= 256) {
			l += 8;
			x >>= 8;
		}
		return l + log_2[(int)x];
	}

	public static int luaO_rawequalObj(TValue t1, TValue t2) {
		if (ttype(t1) != ttype(t2)) {
			return 0;
		}
		else {
			switch (ttype(t1)) {
				case Lua.LUA_TNIL: {
						return 1;
					}
				case Lua.LUA_TNUMBER: {
						return LuaConf.luai_numeq(nvalue(t1), nvalue(t2)) ? 1 : 0;
					}
				case Lua.LUA_TBOOLEAN: {
						return bvalue(t1) == bvalue(t2) ? 1 : 0; // boolean true must be 1....but not in C# !! 
					}
				case Lua.LUA_TLIGHTUSERDATA: {
						return pvalue(t1) == pvalue(t2) ? 1 : 0;
					}
				default: {
						LuaLimits.lua_assert(iscollectable(t1));
						return gcvalue(t1) == gcvalue(t2) ? 1 : 0;
					}
			}
		}
	}

	public static int luaO_str2d(LuaConf.CharPtr s, double[] result) { //lua_Number - out
		LuaConf.CharPtr[] endptr = new LuaConf.CharPtr[1];
		endptr[0] = new LuaConf.CharPtr();
		result[0] = LuaConf.lua_str2number(s, endptr); //out
		if (LuaConf.CharPtr.isEqual(endptr[0], s)) {
			return 0; // conversion failed 
		}
		if (endptr[0].get(0) == 'x' || endptr[0].get(0) == 'X') { // maybe an hexadecimal constant? 
			result[0] = LuaLimits.cast_num(LuaConf.strtoul(s, endptr, 16)); //out
		}
		if (endptr[0].get(0) == '\0') {
			return 1; // most common case 
		}
		while (LuaConf.isspace(endptr[0].get(0))) {
			endptr[0] = endptr[0].next();
		}
		if (endptr[0].get(0) != '\0') {
			return 0; // invalid trailing characters? 
		}
		return 1;
	}

	private static void pushstr(lua_State L, LuaConf.CharPtr str) {
		setsvalue2s(L, L.top, LuaString.luaS_new(L, str));
		LuaDo.incr_top(L);
	}

	// this function handles only `%d', `%c', %f, %p, and `%s' formats 
	public static LuaConf.CharPtr luaO_pushvfstring(lua_State L, LuaConf.CharPtr fmt, Object... argp) {
		int parm_index = 0;
		int n = 1;
		pushstr(L, LuaConf.CharPtr.toCharPtr(""));
		for (;;) {
			LuaConf.CharPtr e = LuaConf.strchr(fmt, '%');
			if (LuaConf.CharPtr.isEqual(e, null)) {
				break;
			}
			setsvalue2s(L, L.top, LuaString.luaS_newlstr(L, fmt, LuaConf.CharPtr.minus(e, fmt))); //(uint)
			LuaDo.incr_top(L);
			switch (e.get(1)) {
				case 's': {
						Object o = argp[parm_index++];
						LuaConf.CharPtr s = (LuaConf.CharPtr)((o instanceof LuaConf.CharPtr) ? o : null);
						if (LuaConf.CharPtr.isEqual(s, null)) {
							s = LuaConf.CharPtr.toCharPtr((String)o);
						}
						if (LuaConf.CharPtr.isEqual(s, null)) {
							s = LuaConf.CharPtr.toCharPtr("(null)");
						}
						pushstr(L, s);
						break;
					}
				case 'c': {
					LuaConf.CharPtr buff = LuaConf.CharPtr.toCharPtr(new char[2]);
						buff.set(0, (char)((Integer)argp[parm_index++]).intValue());
						buff.set(1, '\0');
						pushstr(L, buff);
						break;
					}
				case 'd': {
						setnvalue(L.top, ((Integer)argp[parm_index++]).intValue());
						LuaDo.incr_top(L);
						break;
					}
				case 'f': {
						setnvalue(L.top, ((Double)argp[parm_index++]).doubleValue()); //l_uacNumber
						LuaDo.incr_top(L);
						break;
					}
				case 'p': {
						//CharPtr buff = new char[4*sizeof(void *) + 8]; /* should be enough space for a `%p' */
						LuaConf.CharPtr buff = LuaConf.CharPtr.toCharPtr(new char[32]);
						LuaConf.sprintf(buff, LuaConf.CharPtr.toCharPtr("0x%08x"), argp[parm_index++].hashCode());
						pushstr(L, buff);
						break;
					}
				case '%': {
						pushstr(L, LuaConf.CharPtr.toCharPtr("%"));
						break;
					}
				default: {
						LuaConf.CharPtr buff = LuaConf.CharPtr.toCharPtr(new char[3]);
						buff.set(0, '%');
						buff.set(1, e.get(1));
						buff.set(2, '\0');
						pushstr(L, buff);
						break;
					}
			}
			n += 2;
			fmt = LuaConf.CharPtr.plus(e, 2);
		}
		pushstr(L, fmt);
		LuaVM.luaV_concat(L, n + 1, LuaLimits.cast_int(TValue.minus(L.top, L.base_)) - 1);
		L.top = TValue.minus(L.top, n);
		return svalue(TValue.minus(L.top, 1));
	}

	public static LuaConf.CharPtr luaO_pushfstring(lua_State L, LuaConf.CharPtr fmt, Object... args) {
		return luaO_pushvfstring(L, fmt, args);
	}

	public static void luaO_chunkid(LuaConf.CharPtr out_, LuaConf.CharPtr source, int bufflen) { //uint
		//out_ = "";
		if (source.get(0) == '=') {
			LuaConf.strncpy(out_, LuaConf.CharPtr.plus(source, 1), bufflen); // remove first char  - (int)
			out_.set(bufflen - 1, '\0'); // ensures null termination 
		}
		else {
			// out = "source", or "...source" 
			if (source.get(0) == '@') {
				int l; //uint
				source = source.next(); // skip the `@' 
				bufflen -= ((new String(" '...' ")).length() + 1); //FIXME: - (uint)
				l = LuaConf.strlen(source); //(uint)
				LuaConf.strcpy(out_, LuaConf.CharPtr.toCharPtr(""));
				if (l > bufflen) {
					source = LuaConf.CharPtr.plus(source, (l - bufflen)); // get last part of file name 
					LuaConf.strcat(out_, LuaConf.CharPtr.toCharPtr("..."));
				}
				LuaConf.strcat(out_, source);
			}
			else {
				// out = [string "string"] 
				int len = LuaConf.strcspn(source, LuaConf.CharPtr.toCharPtr("\n\r")); // stop at first newline  - uint
				bufflen -= ((new String(" [string \"...\"] ")).length() + 1); //(uint)
				if (len > bufflen) {
					len = bufflen;
				}
				LuaConf.strcpy(out_, LuaConf.CharPtr.toCharPtr("[string \""));
				if (source.get(len) != '\0') {
					// must truncate? 
					LuaConf.strncat(out_, source, (int)len);
					LuaConf.strcat(out_, LuaConf.CharPtr.toCharPtr("..."));
				}
				else {
					LuaConf.strcat(out_, source);
				}
				LuaConf.strcat(out_, LuaConf.CharPtr.toCharPtr("\"]"));
			}
		}
	}
}