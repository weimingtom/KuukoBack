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

	public static interface ArrayElement 
	{
		void set_index(int index);
		void set_array(Object array);
	}	
	
    /*
     ** Common Header for all collectable objects (in macro form, to be
     ** included in other objects)
     */
    public static class CommonHeader
    {
    	public LuaState.GCObject next;
    	public byte tt; /*Byte*/ /*lu_byte*/
    	public byte marked; /*Byte*/ /*lu_byte*/
    }	
	
	/*
	 ** Common header in struct form
	 */
	public static class GCheader extends LuaObject.CommonHeader {
		
	}    
	
	/*
	 ** Union of all Lua values
	 */
	public static class Value /*struct ValueCls*/
	{
		public LuaState.GCObject gc;
		public Object p;
		public double n;  /*Double*/ /*lua_Number*/
		public int b;

        public Value()
        {

        }

        public Value(Value copy)
        {
            this.gc = copy.gc;
            this.p = copy.p;
            this.n = copy.n;
            this.b = copy.b;
        }

        public void copyFrom(Value copy)
        {
            this.gc = copy.gc;
            this.p = copy.p;
            this.n = copy.n;
            this.b = copy.b;
        }
	}
	
//        
//		 ** Tagged Values
//		 

	///#define TValuefields	Value value; int tt

	public static class TValue implements LuaObject.ArrayElement 
	{
		private TValue[] values = null;
		private int index = -1;
		
		public Value value = new Value();
		public int tt;
		
		public void set_index(int index)
		{
			this.index = index;
		}
		
		public void set_array(Object array) 
		{
			this.values = (TValue[])array;
			ClassType.Assert(this.values != null);
		}

        //TValue this[int offset] get
        public TValue get(int offset)
        {
            return this.values[this.index + offset];
        }

        //TValue this[uint offset] get
		//public TValue get(uint offset)
		//{
		//	return this.values[this.index + (int)offset];
		//}
		
        public static TValue plus(TValue value, int offset)
        {
            return value.values[value.index + offset];
        }

        //operator +
        public static TValue plus(int offset, TValue value)
        {
            return value.values[value.index + offset];
        }

        public static TValue minus(TValue value, int offset)
        {
            return value.values[value.index - offset];
        }

        //operator -
        public static int minus(TValue value, TValue[] array)
        {
            ClassType.Assert(value.values == array);
            return value.index;
        }

        //operator -
        public static int minus(TValue a, TValue b)
        {
            ClassType.Assert(a.values == b.values);
            return a.index - b.index;
        }

        //operator <
		public static boolean lessThan(TValue a, TValue b) 
		{
			ClassType.Assert(a.values == b.values);
			return a.index < b.index;
		}

        //operator <=
		public static boolean lessEqual(TValue a, TValue b) 
        {
            ClassType.Assert(a.values == b.values);
            return a.index <= b.index;
        }

        //operator >
		public static boolean greaterThan(TValue a, TValue b) 
        {
            ClassType.Assert(a.values == b.values);
            return a.index > b.index;
        }

        //operator >=
		public static boolean greaterEqual(TValue a, TValue b) 
        {
            ClassType.Assert(a.values == b.values);
            return a.index >= b.index;
        }
        
        public static TValue inc(/*ref*/ TValue[] value)
		{
        	value[0] = value[0].get(1);
        	return value[0].get(-1);
		}
		
        public static TValue dec(/*ref*/ TValue[] value)
		{
			value[0] = value[0].get(-1);
			return value[0].get(1);
		}
		
        //implicit operator int
		public static int toInt(TValue value)
		{
			return value.index;
		}
		
		public TValue()
		{
			this.values = null;
			this.index = 0;
			this.value = new Value();
			this.tt = 0;
		}
		
		public TValue(TValue value)
		{
			this.values = value.values;
			this.index = value.index;
			this.value = new Value(value.value); // todo: do a shallow copy here
			this.tt = value.tt;
		}
		
		//public TValue(TValue[] values)
		//{
		//	this.values = values;
		//	this.index = Array.IndexOf(values, this);
		//	this.value = new Value();
		//	this.tt = 0;
		//}
		
		public TValue(Value value, int tt)
		{
			this.values = null;
			this.index = 0;
			this.value = new Value(value);
			this.tt = tt;
		}
		
		//public TValue(TValue[] values, Value valueCls, int tt)
		//{
		//	this.values = values;
		//	this.index = Array.IndexOf(values, this);
		//	this.value = new Value(valueCls);
		//	this.tt = tt;
        //}
	}
	
	
	
	
	
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

	public static LuaState.GCObject gcvalue(TValue o) {
		return (LuaState.GCObject)LuaLimits.check_exp(iscollectable(o), o.value.gc);
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

	public static LuaState.lua_State thvalue(TValue o) {
		return (LuaState.lua_State)LuaLimits.check_exp(ttisthread(o), o.value.gc.getTh());
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

	public static void checkliveness(LuaState.global_State g, TValue obj) {
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

	public static void setsvalue(LuaState.lua_State L, TValue obj, LuaState.GCObject x) {
		obj.value.gc = x;
		obj.tt = Lua.LUA_TSTRING;
		checkliveness(LuaState.G(L), obj);
	}

	public static void setuvalue(LuaState.lua_State L, TValue obj, LuaState.GCObject x) {
		obj.value.gc = x;
		obj.tt = Lua.LUA_TUSERDATA;
		checkliveness(LuaState.G(L), obj);
	}

	public static void setthvalue(LuaState.lua_State L, TValue obj, LuaState.GCObject x) {
		obj.value.gc = x;
		obj.tt = Lua.LUA_TTHREAD;
		checkliveness(LuaState.G(L), obj);
	}

	public static void setclvalue(LuaState.lua_State L, TValue obj, Closure x) {
		obj.value.gc = x;
		obj.tt = Lua.LUA_TFUNCTION;
		checkliveness(LuaState.G(L), obj);
	}

	public static void sethvalue(LuaState.lua_State L, TValue obj, Table x) {
		obj.value.gc = x;
		obj.tt = Lua.LUA_TTABLE;
		checkliveness(LuaState.G(L), obj);
	}

	public static void setptvalue(LuaState.lua_State L, TValue obj, Proto x) {
		obj.value.gc = x;
		obj.tt = LUA_TPROTO;
		checkliveness(LuaState.G(L), obj);
	}

	public static void setobj(LuaState.lua_State L, TValue obj1, TValue obj2) {
		obj1.value.copyFrom(obj2.value);
		obj1.tt = obj2.tt;
		checkliveness(LuaState.G(L), obj1);
	}


//        
//		 ** different types of sets, according to destination
//		 

	// from stack to (same) stack 
	///#define setobjs2s	setobj
	public static void setobjs2s(LuaState.lua_State L, TValue obj, TValue x) {
		setobj(L, obj, x);
	}
	//to stack (not from same stack)

	///#define setobj2s	setobj
	public static void setobj2s(LuaState.lua_State L, TValue obj, TValue x) {
		setobj(L, obj, x);
	}

	///#define setsvalue2s	setsvalue
	public static void setsvalue2s(LuaState.lua_State L, TValue obj, TString x) {
		setsvalue(L, obj, x);
	}

	///#define sethvalue2s	sethvalue
	public static void sethvalue2s(LuaState.lua_State L, TValue obj, Table x) {
		sethvalue(L, obj, x);
	}

	///#define setptvalue2s	setptvalue
	public static void setptvalue2s(LuaState.lua_State L, TValue obj, Proto x) {
		setptvalue(L, obj, x);
	}

	// from table to same table 
	///#define setobjt2t	setobj
	public static void setobjt2t(LuaState.lua_State L, TValue obj, TValue x) {
		setobj(L, obj, x);
	}

	// to table 
	///#define setobj2t	setobj
	public static void setobj2t(LuaState.lua_State L, TValue obj, TValue x) {
		setobj(L, obj, x);
	}

	// to new object 
	///#define setobj2n	setobj
	public static void setobj2n(LuaState.lua_State L, TValue obj, TValue x) {
		setobj(L, obj, x);
	}

	///#define setsvalue2n	setsvalue
	public static void setsvalue2n(LuaState.lua_State L, TValue obj, TString x) {
		setsvalue(L, obj, x);
	}

	public static void setttype(TValue obj, int tt) {
		obj.tt = tt;
	}

	public static boolean iscollectable(TValue o) {
		return (ttype(o) >= Lua.LUA_TSTRING);
	}

	public static CLib.CharPtr getstr(TString ts) {
		return ts.str;
	}

	public static CLib.CharPtr svalue(TValue o) { //StkId
		return getstr(rawtsvalue(o));
	}

	public static class Udata_uv extends LuaState.GCObject 
	{
		public LuaObject.Table metatable;
		public LuaObject.Table env;
		public int len; /*uint*/
	}
	
	public static class Udata extends LuaObject.Udata_uv {
		public /*new*/  LuaObject.Udata_uv uv;
		
		//public L_Umaxalign dummy;  /* ensures maximum alignment for `local' udata */
		
		// in the original C code this was allocated alongside the structure memory. it would probably
		// be possible to still do that by allocating memory and pinning it down, but we can do the
		// same thing just as easily by allocating a seperate byte array for it instead.
		public Object user_data;
		
		public Udata() 
		{ 
			this.uv = this; 
		}
	}
	
	//typedef TValue *StkId;  /* index to stack elements */

	/*
	 ** String headers for string table
	 */
	public static class TString_tsv extends LuaState.GCObject 
	{
		public byte reserved;  /*Byte*/ /*lu_byte*/
		/*FIXME:*/
		public long hash; /*int*//*uint*/
		public int len; /*uint*/
	}
	
	public static class TString extends LuaObject.TString_tsv 
	{
		public CLib.CharPtr str;
		
		//public L_Umaxalign dummy;  /* ensures maximum alignment for strings */

        public LuaObject.TString_tsv getTsv()
        {
            return this;
        }

		public TString()
		{
			
		}
		
		public TString(CLib.CharPtr str) 
		{ 
			this.str = str; 
		}
		
		@Override
		public String toString() 
		{
			return str.toString();
		} // for debugging
	}
	
	/*
	 ** Function Prototypes
	 */
	public static class Proto extends LuaState.GCObject 
	{
		public Proto[] protos = null;
		public int index = 0;
		
		public TValue[] k;  /* constants used by the function */
		public long[]/*UInt32[]*//*Instruction[]*/ code;
		public /*new*/ Proto[] p;  /* functions defined inside the function */
		public int[] lineinfo;  /* map from opcodes to source lines */
		public LuaObject.LocVar[] locvars;  /* information about local variables */
		public TString[] upvalues;  /* upvalue names */
		public TString source;
		public int sizeupvalues;
		public int sizek;  /* size of `k' */
		public int sizecode;
		public int sizelineinfo;
		public int sizep;  /* size of `p' */
		public int sizelocvars;
		public int linedefined;
		public int lastlinedefined;
		public LuaState.GCObject gclist;
		public byte nups;  /*Byte*/ /*lu_byte*/ /* number of upvalues */
		public byte numparams;  /*Byte*/ /*lu_byte*/ 
		public byte is_vararg;  /*Byte*/ /*lu_byte*/
		public byte maxstacksize;  /*Byte*/ /*lu_byte*/

        //Proto this[int offset] get
		public Proto get(int offset) 
		{ 
			return this.protos[this.index + offset]; 
		}
	}
	
	// masks for new-style vararg 
	public static final int VARARG_HASARG = 1;
	public static final int VARARG_ISVARARG = 2;
	public static final int VARARG_NEEDSARG = 4;

	public static class LocVar {
		public TString varname;
		public int startpc;  /* first point where variable is active */
		public int endpc;    /* first point where variable is dead */
	}
	
	/*
	 ** Upvalues
	 */
	public static class UpVal extends LuaState.GCObject 
	{
		public static class _u 
		{			
			public static class _l 
			{  
				/* double linked list (when open) */
				public UpVal prev;
				public UpVal next;
			}
			
			public LuaObject.TValue value = new LuaObject.TValue();  /* the value (when closed) */
			public _l l = new _l();
		}
		public /*new*/ _u u = new _u();
		
		public LuaObject.TValue v;  /* points to stack or to its own value */
	}
	
    /*
     ** Closures
     */ 
	public static class ClosureHeader extends LuaState.GCObject {
    	public byte isC; /*Byte*/ /*lu_byte*/
    	public byte nupvalues; /*Byte*/ /*lu_byte*/
    	public LuaState.GCObject gclist;
    	public Table env;
    }
	
	public static class ClosureType {
		private LuaObject.ClosureHeader header;

        //implicit operator ClosureHeader
		public static LuaObject.ClosureHeader toClosureHeader(ClosureType ctype) 
		{
            return ctype.header; 
		}
		
		public ClosureType(LuaObject.ClosureHeader header) 
		{ 
			this.header = header; 
		}

        /*Byte*/ /*lu_byte*/
        public byte getIsC()
        {
            return header.isC;
        }

        /*Byte*/ /*lu_byte*/
        public void setIsC(byte val) 
        {
            header.isC = val;
        }

        /*Byte*/
        /*lu_byte*/
        public byte getNupvalues()
        {
            return header.nupvalues;
        }

        /*Byte*/
        /*lu_byte*/
        public void setNupvalues(byte val)
        {
            header.nupvalues = val;
        }

        public LuaState.GCObject getGclist()
        {
            return header.gclist;
        }

        public void setGclist(LuaState.GCObject val)
        {
            header.gclist = val;
        }

        public Table getEnv()
        {
            return header.env;
        }

        public void setEnv(Table val)
        {
            header.env = val;
        }
	}
	
	
	public static class CClosure extends ClosureType {
		public Lua.lua_CFunction f;
		public TValue[] upvalue;
	
		public CClosure(ClosureHeader header) 
		{
			super(header);
						
		}
	}	
	
	public static class LClosure extends LuaObject.ClosureType {	
		public Proto p;
		public UpVal[] upvals;
		
		public LClosure(LuaObject.ClosureHeader header) 
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

	/*
	 ** Tables
	 */
	public static class TKey_nk extends TValue 
	{
		public LuaObject.Node next;  /* for chaining */
		
		public TKey_nk() 
		{
			
		}
		
		public TKey_nk(Value value, int tt, LuaObject.Node next)
		{
			super(new Value(value), tt);
			this.next = next;
		}
	}
	
	public static class TKey
	{
		public LuaObject.TKey_nk nk = new LuaObject.TKey_nk();
		
		public TKey()
		{
			this.nk = new LuaObject.TKey_nk();
		}
		
		public TKey(TKey copy)
		{
			this.nk = new LuaObject.TKey_nk(new Value(copy.nk.value), copy.nk.tt, copy.nk.next);
		}
		
		public TKey(Value value, int tt, LuaObject.Node next)
		{
			this.nk = new LuaObject.TKey_nk(new Value(value), tt, next);
		}

        public TValue getTvk()
        {
            return this.nk;
        }
	}
	
	public static class Node implements LuaObject.ArrayElement {
		private Node[] values = null;
		private int index = -1;
		
		public static int ids = 0;
		public int id = ids++;
		
		public TValue i_val;
		public TKey i_key;
		
		public void set_index(int index)
		{
			this.index = index;
		}
		
		public void set_array(Object array) 
		{
			this.values = (Node[])array;
			ClassType.Assert(this.values != null);
		}
		
		public Node()
		{
			this.i_val = new TValue();
			this.i_key = new TKey();
		}
		
		public Node(Node copy)
		{
			this.values = copy.values;
			this.index = copy.index;
			this.i_val = new TValue(copy.i_val);
			this.i_key = new TKey(copy.i_key);
		}
		
		public Node(TValue i_val, TKey i_key)
		{
			this.values = new Node[] { this };
			this.index = 0;
			this.i_val = i_val;
			this.i_key = i_key;
		}

        //Node this[int offset]
        public Node get(int offset)
        {
            return this.values[this.index + offset];
        }

        //Node this[uint offset]
        //public Node get(uint offset)
        //{
        //    return this.values[this.index + (int)offset];
        //}

        //operator -
		public static int minus(Node n1, Node n2)
		{
			ClassType.Assert(n1.values == n2.values);
			return n1.index - n2.index;
		}
		
		public static Node inc(/*ref*/ Node[] node)
		{
			node[0] = node[0].get(1);
			return node[0].get(-1);
		}
		
		public static Node dec(/*ref*/ Node[] node)
		{
			node[0] = node[0].get(-1);
			return node[0].get(1);
		}
		
        //operator >
		public static boolean greaterThan(Node n1, Node n2) 
		{
			ClassType.Assert(n1.values == n2.values); 
			return n1.index > n2.index;
		}
		
        //operator >=
		public static boolean greaterEqual(Node n1, Node n2) 
		{ 
			ClassType.Assert(n1.values == n2.values); 
			return n1.index >= n2.index;
		}

        //operator <
		public static boolean lessThan(Node n1, Node n2) 
		{ 
			ClassType.Assert(n1.values == n2.values); 
			return n1.index < n2.index;
		}
		
        //operator <=
		public static boolean lessEqual(Node n1, Node n2) 
		{ 
			ClassType.Assert(n1.values == n2.values); 
			return n1.index <= n2.index; 
		}

        //operator ==
		public static boolean isEqual(Node n1, Node n2) 
        {
			Object o1 = (Node)((n1 instanceof Node) ? n1 : null);
			Object o2 = (Node)((n2 instanceof Node) ? n2 : null);
            if ((o1 == null) && (o2 == null))
            {
                return true;
            }
            if (o1 == null)
            {
                return false;
            }
            if (o2 == null)
            {
                return false;
            }
            if (n1.values != n2.values)
            {
                return false;
            }
            return n1.index == n2.index;
        }

        //operator !=
		public static boolean isNotEqual(Node n1, Node n2) 
        {
            //return !(n1 == n2); 
            return !isEqual(n1, n2);
        }

		@Override
		public boolean equals(Object o) 
		{ 
			//return this == (Node)o; 
            return Node.isEqual(this, (Node)o);
		}
		
		@Override
		public int hashCode() 
		{ 
			return 0; 
		}
	}
	
	
	
	
	
	
	
	
	
	public static class Table extends LuaState.GCObject 
	{
		public byte flags; /*Byte*/ /*lu_byte*/  /* 1<<p means tagmethod(p) is not present */
		public byte lsizenode;  /*Byte*/ /*lu_byte*/ /* log2 of size of `node' array */
		public Table metatable;
		public TValue[] array;  /* array part */
		public LuaObject.Node[] node;
		public int lastfree;  /* any free position is before this position */
		public LuaState.GCObject gclist;
		public int sizearray;  /* size of `array' array */
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

	public static int luaO_str2d(CLib.CharPtr s, double[] result) { //lua_Number - out
		CLib.CharPtr[] endptr = new CLib.CharPtr[1];
		endptr[0] = new CLib.CharPtr();
		result[0] = LuaConf.lua_str2number(s, endptr); //out
		if (CLib.CharPtr.isEqual(endptr[0], s)) {
			return 0; // conversion failed 
		}
		if (endptr[0].get(0) == 'x' || endptr[0].get(0) == 'X') { // maybe an hexadecimal constant? 
			result[0] = LuaLimits.cast_num(CLib.strtoul(s, endptr, 16)); //out
		}
		if (endptr[0].get(0) == '\0') {
			return 1; // most common case 
		}
		while (CLib.isspace(endptr[0].get(0))) {
			endptr[0] = endptr[0].next();
		}
		if (endptr[0].get(0) != '\0') {
			return 0; // invalid trailing characters? 
		}
		return 1;
	}

	private static void pushstr(LuaState.lua_State L, CLib.CharPtr str) {
		setsvalue2s(L, L.top, LuaString.luaS_new(L, str));
		LuaDo.incr_top(L);
	}

	// this function handles only `%d', `%c', %f, %p, and `%s' formats 
	public static CLib.CharPtr luaO_pushvfstring(LuaState.lua_State L, CLib.CharPtr fmt, Object... argp) {
		int parm_index = 0;
		int n = 1;
		pushstr(L, CLib.CharPtr.toCharPtr(""));
		for (;;) {
			CLib.CharPtr e = CLib.strchr(fmt, '%');
			if (CLib.CharPtr.isEqual(e, null)) {
				break;
			}
			setsvalue2s(L, L.top, LuaString.luaS_newlstr(L, fmt, CLib.CharPtr.minus(e, fmt))); //(uint)
			LuaDo.incr_top(L);
			switch (e.get(1)) {
				case 's': {
						Object o = argp[parm_index++];
						CLib.CharPtr s = (CLib.CharPtr)((o instanceof CLib.CharPtr) ? o : null);
						if (CLib.CharPtr.isEqual(s, null)) {
							s = CLib.CharPtr.toCharPtr((String)o);
						}
						if (CLib.CharPtr.isEqual(s, null)) {
							s = CLib.CharPtr.toCharPtr("(null)");
						}
						pushstr(L, s);
						break;
					}
				case 'c': {
					CLib.CharPtr buff = CLib.CharPtr.toCharPtr(new char[2]);
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
						CLib.CharPtr buff = CLib.CharPtr.toCharPtr(new char[32]);
						CLib.sprintf(buff, CLib.CharPtr.toCharPtr("0x%08x"), argp[parm_index++].hashCode());
						pushstr(L, buff);
						break;
					}
				case '%': {
						pushstr(L, CLib.CharPtr.toCharPtr("%"));
						break;
					}
				default: {
						CLib.CharPtr buff = CLib.CharPtr.toCharPtr(new char[3]);
						buff.set(0, '%');
						buff.set(1, e.get(1));
						buff.set(2, '\0');
						pushstr(L, buff);
						break;
					}
			}
			n += 2;
			fmt = CLib.CharPtr.plus(e, 2);
		}
		pushstr(L, fmt);
		LuaVM.luaV_concat(L, n + 1, LuaLimits.cast_int(TValue.minus(L.top, L.base_)) - 1);
		L.top = TValue.minus(L.top, n);
		return svalue(TValue.minus(L.top, 1));
	}

	public static CLib.CharPtr luaO_pushfstring(LuaState.lua_State L, CLib.CharPtr fmt, Object... args) {
		return luaO_pushvfstring(L, fmt, args);
	}

	public static void luaO_chunkid(CLib.CharPtr out_, CLib.CharPtr source, int bufflen) { //uint
		//out_ = "";
		if (source.get(0) == '=') {
			CLib.strncpy(out_, CLib.CharPtr.plus(source, 1), bufflen); // remove first char  - (int)
			out_.set(bufflen - 1, '\0'); // ensures null termination 
		}
		else {
			// out = "source", or "...source" 
			if (source.get(0) == '@') {
				int l; //uint
				source = source.next(); // skip the `@' 
				bufflen -= ((new String(" '...' ")).length() + 1); //FIXME: - (uint)
				l = CLib.strlen(source); //(uint)
				CLib.strcpy(out_, CLib.CharPtr.toCharPtr(""));
				if (l > bufflen) {
					source = CLib.CharPtr.plus(source, (l - bufflen)); // get last part of file name 
					CLib.strcat(out_, CLib.CharPtr.toCharPtr("..."));
				}
				CLib.strcat(out_, source);
			}
			else {
				// out = [string "string"] 
				int len = CLib.strcspn(source, CLib.CharPtr.toCharPtr("\n\r")); // stop at first newline  - uint
				bufflen -= ((new String(" [string \"...\"] ")).length() + 1); //(uint)
				if (len > bufflen) {
					len = bufflen;
				}
				CLib.strcpy(out_, CLib.CharPtr.toCharPtr("[string \""));
				if (source.get(len) != '\0') {
					// must truncate? 
					CLib.strncat(out_, source, (int)len);
					CLib.strcat(out_, CLib.CharPtr.toCharPtr("..."));
				}
				else {
					CLib.strcat(out_, source);
				}
				CLib.strcat(out_, CLib.CharPtr.toCharPtr("\"]"));
			}
		}
	}
}