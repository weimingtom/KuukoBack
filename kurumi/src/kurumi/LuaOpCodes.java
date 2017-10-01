package kurumi;

//
// ** $Id: lopcodes.c,v 1.37.1.1 2007/12/27 13:02:25 roberto Exp $
// ** See Copyright Notice in lua.h
// 

//using lu_byte = System.Byte;
//using Instruction = System.UInt32;

public class LuaOpCodes {
//        ===========================================================================
//		  We assume that instructions are unsigned numbers.
//		  All instructions have an opcode in the first 6 bits.
//		  Instructions can have the following fields:
//			`A' : 8 bits
//			`B' : 9 bits
//			`C' : 9 bits
//			`Bx' : 18 bits (`B' and `C' together)
//			`sBx' : signed Bx
//
//		  A signed argument is represented in excess K; that is, the number
//		  value is the unsigned value minus K. K is exactly the maximum value
//		  for that argument (so that -max is represented by 0, and +max is
//		  represented by 2*max), which is half the maximum for the corresponding
//		  unsigned argument.
//		===========================================================================

	public static enum OpMode 
	{ 
		/* basic instruction format */
		iABC, 
		iABx, 
		iAsBx;

		public int getValue() {
			return this.ordinal();
		}

		public static OpMode forValue(int value) {
			return values()[value];
		}
	}

//        
//		 ** size and position of opcode arguments.
//		 
	public static final int SIZE_C = 9;
	public static final int SIZE_B = 9;
	public static final int SIZE_Bx = (SIZE_C + SIZE_B);
	public static final int SIZE_A = 8;

	public static final int SIZE_OP = 6;

	public static final int POS_OP = 0;
	public static final int POS_A = (POS_OP + SIZE_OP);
	public static final int POS_C = (POS_A + SIZE_A);
	public static final int POS_B = (POS_C + SIZE_C);
	public static final int POS_Bx = POS_C;

//        
//		 ** limits for opcode arguments.
//		 ** we use (signed) int to manipulate most arguments,
//		 ** so they must fit in LUAI_BITSINT-1 bits (-1 for sign)
//		 
	///#if SIZE_Bx < LUAI_BITSINT-1
	public static final int MAXARG_Bx = ((1<<SIZE_Bx)-1);
	public static final int MAXARG_sBx = (MAXARG_Bx>>1); // `sBx' is signed 
	///#else
	//public const int MAXARG_Bx			= System.Int32.MaxValue;
	//public const int MAXARG_sBx			= System.Int32.MaxValue;
	///#endif

	//public const uint MAXARG_A = (uint)((1 << (int)SIZE_A) -1);
	//public const uint MAXARG_B = (uint)((1 << (int)SIZE_B) -1);
	//public const uint MAXARG_C = (uint)((1 << (int)SIZE_C) -1);
	public static final long MAXARG_A = (long)(((1 << (int)SIZE_A) -1) & 0xffffffff);
	public static final long MAXARG_B = (long)(((1 << (int)SIZE_B) -1) & 0xffffffff);
	public static final long MAXARG_C = (long)(((1 << (int)SIZE_C) -1) & 0xffffffff);

	// creates a mask with `n' 1 bits at position `p' 
	//public static int MASK1(int n, int p) { return ((~((~(Instruction)0) << n)) << p); }
	public static long MASK1(int n, int p) { //uint
		//return (uint)((~((~0) << n)) << p); 
		return (long)(((~((~0) << n)) << p) & 0xffffffff);
	}

	// creates a mask with `n' 0 bits at position `p' 
	public static long MASK0(int n, int p) { //uint
		//return (uint)(~MASK1(n, p)); 
		return (long)((~MASK1(n, p)) & 0xffffffff);
	}

//        
//		 ** the following macros help to manipulate instructions
//		 
	public static OpCode GET_OPCODE(long i) { //Instruction - UInt32
		return (OpCode)longToOpCode((i >> POS_OP) & MASK1(SIZE_OP, 0));
	}

	public static OpCode GET_OPCODE(LuaCode.InstructionPtr i) {
		return GET_OPCODE(i.get(0));
	}

	public static void SET_OPCODE(long[] i, long o) { //Instruction - UInt32 - Instruction - UInt32 - ref
		i[0] = (long)(i[0] & MASK0(SIZE_OP, POS_OP)) | ((o << POS_OP) & MASK1(SIZE_OP, POS_OP)); //Instruction - UInt32
	}

	public static void SET_OPCODE(long[] i, OpCode opcode) { //Instruction - UInt32 - ref
		i[0] = (long)(i[0] & MASK0(SIZE_OP, POS_OP)) | (((long)opcode.getValue() << POS_OP) & MASK1(SIZE_OP, POS_OP)); //uint - Instruction - UInt32
	}

	public static void SET_OPCODE(LuaCode.InstructionPtr i, OpCode opcode) {
		long[] c_ref = new long[1];
		c_ref[0] = i.codes[i.pc];
		SET_OPCODE(c_ref, opcode); //ref
		i.codes[i.pc] = c_ref[0];
	}

	public static int GETARG_A(long i) { //Instruction - UInt32
		return (int)((i >> POS_A) & MASK1(SIZE_A, 0));
	}

	public static int GETARG_A(LuaCode.InstructionPtr i) {
		return GETARG_A(i.get(0));
	}

	public static void SETARG_A(LuaCode.InstructionPtr i, int u) {
		i.set(0, (long)((i.get(0) & MASK0(SIZE_A, POS_A)) | ((u << POS_A) & MASK1(SIZE_A, POS_A)))); //Instruction - UInt32
	}

	public static int GETARG_B(long i) { //Instruction - UInt32
		return (int)((i >> POS_B) & MASK1(SIZE_B, 0));
	}

	public static int GETARG_B(LuaCode.InstructionPtr i) {
		return GETARG_B(i.get(0));
	}

	public static void SETARG_B(LuaCode.InstructionPtr i, int b) {
		i.set(0, (long)((i.get(0) & MASK0(SIZE_B, POS_B)) | ((b << POS_B) & MASK1(SIZE_B, POS_B)))); //Instruction - UInt32
	}

	public static int GETARG_C(long i) { //Instruction - UInt32
		return (int)((i>>POS_C) & MASK1(SIZE_C, 0));
	}

	public static int GETARG_C(LuaCode.InstructionPtr i) {
		return GETARG_C(i.get(0));
	}

	public static void SETARG_C(LuaCode.InstructionPtr i, int b) {
		i.set(0, (long)((i.get(0) & MASK0(SIZE_C, POS_C)) | ((b << POS_C) & MASK1(SIZE_C, POS_C)))); //Instruction - UInt32
	}

	public static int GETARG_Bx(long i) { //Instruction - UInt32
		return (int)((i>>POS_Bx) & MASK1(SIZE_Bx, 0));
	}

	public static int GETARG_Bx(LuaCode.InstructionPtr i) {
		return GETARG_Bx(i.get(0));
	}

	public static void SETARG_Bx(LuaCode.InstructionPtr i, int b) {
		i.set(0, (long)((i.get(0) & MASK0(SIZE_Bx, POS_Bx)) | ((b << POS_Bx) & MASK1(SIZE_Bx, POS_Bx)))); //Instruction - UInt32
	}

	public static int GETARG_sBx(long i) { //Instruction - UInt32
		return (GETARG_Bx(i) - MAXARG_sBx);
	}

	public static int GETARG_sBx(LuaCode.InstructionPtr i) {
		return GETARG_sBx(i.get(0));
	}

	public static void SETARG_sBx(LuaCode.InstructionPtr i, int b) {
		SETARG_Bx(i, b + MAXARG_sBx);
	}

	//FIXME:long
	public static int CREATE_ABC(OpCode o, int a, int b, int c) {
		return (int)((o.getValue() << POS_OP) | (a << POS_A) | (b << POS_B) | (c << POS_C));
	}

	//FIXME:long
	public static int CREATE_ABx(OpCode o, int a, int bc) {
		int result = (int)((o.getValue() << POS_OP) | (a << POS_A) | (bc << POS_Bx));
		return (int)((o.getValue() << POS_OP) | (a << POS_A) | (bc << POS_Bx));
	}

//        
//		 ** Macros to operate RK indices
//		 

	// this bit 1 means constant (0 means register) 
	public final static int BITRK = (1 << (SIZE_B - 1));

	// test whether value is a constant 
	public static int ISK(int x) {
		return x & BITRK;
	}

	// gets the index of the constant 
	public static int INDEXK(int r) {
		return r & (~BITRK);
	}

	public static final int MAXINDEXRK = BITRK - 1;

	// code a constant index as a RK value 
	public static int RKASK(int x) {
		return x | BITRK;
	}

//        
//		 ** invalid register that fits in 8 bits
//		 
	public static final int NO_REG = (int)MAXARG_A;

//        
//		 ** R(x) - register
//		 ** Kst(x) - constant (in constant table)
//		 ** RK(x) == if ISK(x) then Kst(INDEXK(x)) else R(x)
//		 


	
	public static enum OpCode {
		/*----------------------------------------------------------------------
		name		args	description
		------------------------------------------------------------------------*/
		OP_MOVE(0),/*	A B	R(A) := R(B)					*/
		OP_LOADK(1),/*	A Bx	R(A) := Kst(Bx)					*/
		OP_LOADBOOL(2),/*	A B C	R(A) := (Bool)B; if (C) pc++			*/
		OP_LOADNIL(3),/*	A B	R(A) := ... := R(B) := nil			*/
		OP_GETUPVAL(4),/*	A B	R(A) := UpValue[B]				*/
		
		OP_GETGLOBAL(5),/*	A Bx	R(A) := Gbl[Kst(Bx)]				*/
		OP_GETTABLE(6),/*	A B C	R(A) := R(B)[RK(C)]				*/
		
		OP_SETGLOBAL(7),/*	A Bx	Gbl[Kst(Bx)] := R(A)				*/
		OP_SETUPVAL(8),/*	A B	UpValue[B] := R(A)				*/
		OP_SETTABLE(9),/*	A B C	R(A)[RK(B)] := RK(C)				*/
		
		OP_NEWTABLE(10),/*	A B C	R(A) := {} (size = B,C)				*/
		
		OP_SELF(11),/*	A B C	R(A+1) := R(B); R(A) := R(B)[RK(C)]		*/
		
		OP_ADD(12),/*	A B C	R(A) := RK(B) + RK(C)				*/
		OP_SUB(13),/*	A B C	R(A) := RK(B) - RK(C)				*/
		OP_MUL(14),/*	A B C	R(A) := RK(B) * RK(C)				*/
		OP_DIV(15),/*	A B C	R(A) := RK(B) / RK(C)				*/
		OP_MOD(16),/*	A B C	R(A) := RK(B) % RK(C)				*/
		OP_POW(17),/*	A B C	R(A) := RK(B) ^ RK(C)				*/
		OP_UNM(18),/*	A B	R(A) := -R(B)					*/
		OP_NOT(19),/*	A B	R(A) := not R(B)				*/
		OP_LEN(20),/*	A B	R(A) := length of R(B)				*/
		
		OP_CONCAT(21),/*	A B C	R(A) := R(B).. ... ..R(C)			*/
		
		OP_JMP(22),/*	sBx	pc+=sBx					*/
		
		OP_EQ(23),/*	A B C	if ((RK(B) == RK(C)) ~= A) then pc++		*/
		OP_LT(24),/*	A B C	if ((RK(B) <  RK(C)) ~= A) then pc++  		*/
		OP_LE(25),/*	A B C	if ((RK(B) <= RK(C)) ~= A) then pc++  		*/
		
		OP_TEST(26),/*	A C	if not (R(A) <=> C) then pc++			*/
		OP_TESTSET(27),/*	A B C	if (R(B) <=> C) then R(A) := R(B) else pc++	*/
		
		OP_CALL(28),/*	A B C	R(A), ... ,R(A+C-2) := R(A)(R(A+1), ... ,R(A+B-1)) */
		OP_TAILCALL(29),/*	A B C	return R(A)(R(A+1), ... ,R(A+B-1))		*/
		OP_RETURN(30),/*	A B	return R(A), ... ,R(A+B-2)	(see note)	*/
		
		OP_FORLOOP(31),/*	A sBx	R(A)+=R(A+2);
					if R(A) <?= R(A+1) then { pc+=sBx; R(A+3)=R(A) }*/
		OP_FORPREP(32),/*	A sBx	R(A)-=R(A+2); pc+=sBx				*/
		
		OP_TFORLOOP(33),/*	A C	R(A+3), ... ,R(A+2+C) := R(A)(R(A+1), R(A+2));
								if R(A+3) ~= nil then R(A+2)=R(A+3) else pc++	*/
		OP_SETLIST(34),/*	A B C	R(A)[(C-1)*FPF+i] := R(A+i), 1 <= i <= B	*/
		
		OP_CLOSE(35),/*	A 	close all variables in the stack up to (>=) R(A)*/
		OP_CLOSURE(36),/*	A Bx	R(A) := closure(KPROTO[Bx], R(A), ... ,R(A+n))	*/
		
		OP_VARARG(37);/*	A B	R(A), R(A+1), ..., R(A+B-1) = vararg		*/

		private int intValue;
		private static java.util.HashMap<Integer, OpCode> mappings;
		private synchronized static java.util.HashMap<Integer, OpCode> getMappings() {
			if (mappings == null) {
				mappings = new java.util.HashMap<Integer, OpCode>();
			}
			return mappings;
		}

		private OpCode(int value) {
			intValue = value;
			OpCode.getMappings().put(value, this);
		}

		public int getValue() {
			return intValue;
		}

		public static OpCode forValue(int value) {
			return getMappings().get(value);
		}
	}
	
    public static long opCodeToLong(LuaOpCodes.OpCode code)
    {
        switch (code)
        {
			case OP_MOVE:
                return 0;
			case OP_LOADK:
                return 1;
			case OP_LOADBOOL:
                return 2;
			case OP_LOADNIL:
                return 3;
			case OP_GETUPVAL:
                return 4;
			case OP_GETGLOBAL:
                return 5;
			case OP_GETTABLE:
                return 6;
			case OP_SETGLOBAL:
                return 7;
			case OP_SETUPVAL:
                return 8;
			case OP_SETTABLE:
                return 9;
			case OP_NEWTABLE:
                return 10;
			case OP_SELF:
                return 11;
			case OP_ADD:
                return 12;
			case OP_SUB:
                return 13;
			case OP_MUL:
                return 14;
			case OP_DIV:
                return 15;
			case OP_MOD:
                return 16;
			case OP_POW:
                return 17;
			case OP_UNM:
                return 18;
			case OP_NOT:
                return 19;
			case OP_LEN:
                return 20;
			case OP_CONCAT:
                return 21;
			case OP_JMP:
                return 22;
			case OP_EQ:
                return 23;
			case OP_LT:
                return 24;
			case OP_LE:
                return 25;
			case OP_TEST:
                return 26;
			case OP_TESTSET:
                return 27;
			case OP_CALL:
                return 28;
			case OP_TAILCALL:
                return 29;
			case OP_RETURN:
                return 30;
			case OP_FORLOOP:
                return 31;
			case OP_FORPREP:
                return 32;
			case OP_TFORLOOP:
                return 33;
			case OP_SETLIST:
                return 34;
			case OP_CLOSE:
                return 35;
			case OP_CLOSURE:
                return 36;
			case OP_VARARG:
                return 37;
        }
		throw new RuntimeException("OpCode error");
    }

    public static LuaOpCodes.OpCode longToOpCode(long code)
    {
        switch ((int)code)
        {
            case 0:
                return LuaOpCodes.OpCode.OP_MOVE;
            case 1:
                return LuaOpCodes.OpCode.OP_LOADK;
            case 2:
                return LuaOpCodes.OpCode.OP_LOADBOOL;
            case 3:
                return LuaOpCodes.OpCode.OP_LOADNIL;
            case 4:
                return LuaOpCodes.OpCode.OP_GETUPVAL;
            case 5:
                return LuaOpCodes.OpCode.OP_GETGLOBAL;
            case 6:
                return LuaOpCodes.OpCode.OP_GETTABLE;
            case 7:
                return LuaOpCodes.OpCode.OP_SETGLOBAL;
            case 8:
                return LuaOpCodes.OpCode.OP_SETUPVAL;
            case 9:
                return LuaOpCodes.OpCode.OP_SETTABLE;
            case 10:
                return LuaOpCodes.OpCode.OP_NEWTABLE;
            case 11:
                return LuaOpCodes.OpCode.OP_SELF;
            case 12:
                return LuaOpCodes.OpCode.OP_ADD;
            case 13:
                return LuaOpCodes.OpCode.OP_SUB;
            case 14:
                return LuaOpCodes.OpCode.OP_MUL;
            case 15:
                return LuaOpCodes.OpCode.OP_DIV;
            case 16:
                return LuaOpCodes.OpCode.OP_MOD;
            case 17:
                return LuaOpCodes.OpCode.OP_POW;
            case 18:
                return LuaOpCodes.OpCode.OP_UNM;
            case 19:
                return LuaOpCodes.OpCode.OP_NOT;
            case 20:
                return LuaOpCodes.OpCode.OP_LEN;
            case 21:
                return LuaOpCodes.OpCode.OP_CONCAT;
            case 22:
                return LuaOpCodes.OpCode.OP_JMP;
            case 23:
                return LuaOpCodes.OpCode.OP_EQ;
            case 24:
                return LuaOpCodes.OpCode.OP_LT;
            case 25:
                return LuaOpCodes.OpCode.OP_LE;
            case 26:
                return LuaOpCodes.OpCode.OP_TEST;
            case 27:
                return LuaOpCodes.OpCode.OP_TESTSET;
            case 28:
                return LuaOpCodes.OpCode.OP_CALL;
            case 29:
                return LuaOpCodes.OpCode.OP_TAILCALL;
            case 30:
                return LuaOpCodes.OpCode.OP_RETURN;
            case 31:
                return LuaOpCodes.OpCode.OP_FORLOOP;
            case 32:
                return LuaOpCodes.OpCode.OP_FORPREP;
            case 33:
                return LuaOpCodes.OpCode.OP_TFORLOOP;
            case 34:
                return LuaOpCodes.OpCode.OP_SETLIST;
            case 35:
                return LuaOpCodes.OpCode.OP_CLOSE;
            case 36:
                return LuaOpCodes.OpCode.OP_CLOSURE;
            case 37:
                return LuaOpCodes.OpCode.OP_VARARG;
        }
		throw new RuntimeException("OpCode error");
    }
	
	/*===========================================================================
    Notes:
    (*) In OP_CALL, if (B == 0) then B = top. C is the number of returns - 1,
  	  and can be 0: OP_CALL then sets `top' to last_result+1, so
  	  next open instruction (OP_CALL, OP_RETURN, OP_SETLIST) may use `top'.
  
    (*) In OP_VARARG, if (B == 0) then use actual number of varargs and
  	  set top (like in OP_CALL with C == 0).
  
    (*) In OP_RETURN, if (B == 0) then return up to `top'
  
    (*) In OP_SETLIST, if (B == 0) then B = `top';
  	  if (C == 0) then next `instruction' is real C
  
    (*) For comparisons, A specifies what condition the test should accept
  	  (true or false).
  
    (*) All `skips' (pc++) assume that next instruction is a jump
  	===========================================================================*/
	
	
	/*
	 ** masks for instruction properties. The format is:
	 ** bits 0-1: op mode
	 ** bits 2-3: C arg mode
	 ** bits 4-5: B arg mode
	 ** bit 6: instruction set register A
	 ** bit 7: operator is a test
	 */
	
	public static enum OpArgMask {
		OpArgN,  /* argument is not used */
		OpArgU,  /* argument is used */
		OpArgR,  /* argument is a register or a jump offset */
		OpArgK;   /* argument is a constant or register/constant */

		public int getValue() {
			return this.ordinal();
		}

		public static OpArgMask forValue(int value) {
			return values()[value];
		}
	}
	
//        
//		 ** grep "ORDER OP" if you change these enums
//		 
	public static OpMode getOpMode(OpCode m) {
		switch (luaP_opmodes[m.getValue()] & 3) {
			default:
			case 0:
				return OpMode.iABC;
			case 1:
				return OpMode.iABx;
			case 2:
				return OpMode.iAsBx;
		}
	}

	public static OpArgMask getBMode(OpCode m) {
		switch ((luaP_opmodes[m.getValue()] >> 4) & 3) {
			default:
			case 0:
				return OpArgMask.OpArgN;
			case 1:
				return OpArgMask.OpArgU;
			case 2:
				return OpArgMask.OpArgR;
			case 3:
				return OpArgMask.OpArgK;
		}
	}

	public static OpArgMask getCMode(OpCode m) {
		switch ((luaP_opmodes[m.getValue()] >> 2) & 3) {
			default:
			case 0:
				return OpArgMask.OpArgN;
			case 1:
				return OpArgMask.OpArgU;
			case 2:
				return OpArgMask.OpArgR;
			case 3:
				return OpArgMask.OpArgK;
		}
	}

	public static int testAMode(OpCode m) {
		return luaP_opmodes[m.getValue()] & (1 << 6);
	}

	public static int testTMode(OpCode m) {
		return luaP_opmodes[m.getValue()] & (1 << 7);
	}

	// number of list items to accumulate before a SETLIST instruction 
	public static final int LFIELDS_PER_FLUSH = 50;

	// ORDER OP 
	public final static LuaConf.CharPtr[] luaP_opnames = { 
		LuaConf.CharPtr.toCharPtr("MOVE"), 
		LuaConf.CharPtr.toCharPtr("LOADK"), 
		LuaConf.CharPtr.toCharPtr("LOADBOOL"), 
		LuaConf.CharPtr.toCharPtr("LOADNIL"), 
		LuaConf.CharPtr.toCharPtr("GETUPVAL"), 
		LuaConf.CharPtr.toCharPtr("GETGLOBAL"), 
		LuaConf.CharPtr.toCharPtr("GETTABLE"), 
		LuaConf.CharPtr.toCharPtr("SETGLOBAL"), 
		LuaConf.CharPtr.toCharPtr("SETUPVAL"), 
		LuaConf.CharPtr.toCharPtr("SETTABLE"), 
		LuaConf.CharPtr.toCharPtr("NEWTABLE"), 
		LuaConf.CharPtr.toCharPtr("SELF"), 
		LuaConf.CharPtr.toCharPtr("ADD"), 
		LuaConf.CharPtr.toCharPtr("SUB"), 
		LuaConf.CharPtr.toCharPtr("MUL"), 
		LuaConf.CharPtr.toCharPtr("DIV"), 
		LuaConf.CharPtr.toCharPtr("MOD"), 
		LuaConf.CharPtr.toCharPtr("POW"), 
		LuaConf.CharPtr.toCharPtr("UNM"), 
		LuaConf.CharPtr.toCharPtr("NOT"), 
		LuaConf.CharPtr.toCharPtr("LEN"), 
		LuaConf.CharPtr.toCharPtr("CONCAT"), 
		LuaConf.CharPtr.toCharPtr("JMP"), 
		LuaConf.CharPtr.toCharPtr("EQ"), 
		LuaConf.CharPtr.toCharPtr("LT"),
		LuaConf.CharPtr.toCharPtr("LE"), 
		LuaConf.CharPtr.toCharPtr("TEST"), 
		LuaConf.CharPtr.toCharPtr("TESTSET"), 
		LuaConf.CharPtr.toCharPtr("CALL"), 
		LuaConf.CharPtr.toCharPtr("TAILCALL"), 
		LuaConf.CharPtr.toCharPtr("RETURN"), 
		LuaConf.CharPtr.toCharPtr("FORLOOP"), 
		LuaConf.CharPtr.toCharPtr("FORPREP"), 
		LuaConf.CharPtr.toCharPtr("TFORLOOP"), 
		LuaConf.CharPtr.toCharPtr("SETLIST"), 
		LuaConf.CharPtr.toCharPtr("CLOSE"), 
		LuaConf.CharPtr.toCharPtr("CLOSURE"), 
		LuaConf.CharPtr.toCharPtr("VARARG") 
	};


	private static byte opmode(byte t, byte a, OpArgMask b, OpArgMask c, OpMode m) { //lu_byte - lu_byte - lu_byte
		int bValue = 0;
		int cValue = 0;
		int mValue = 0;
		switch (b) {
			case OpArgN:
				bValue = 0;
				break;
			case OpArgU:
				bValue = 1;
				break;
			case OpArgR:
				bValue = 2;
				break;
			case OpArgK:
				bValue = 3;
				break;
		}
		switch (c) {
			case OpArgN:
				cValue = 0;
				break;
			case OpArgU:
				cValue = 1;
				break;
			case OpArgR:
				cValue = 2;
				break;
			case OpArgK:
				cValue = 3;
				break;
		}
		switch (m) {
			case iABC:
				mValue = 0;
				break;
			case iABx:
				mValue = 1;
				break;
			case iAsBx:
				mValue = 2;
				break;
		}
		return (byte)(((t) << 7) | ((a) << 6) | (((byte)bValue) << 4) | (((byte)cValue) << 2) | ((byte)mValue)); //lu_byte - lu_byte - lu_byte - lu_byte
	}

	//       T  A    B       C     mode		   opcode		
	//lu_byte[]
	private final static byte[] luaP_opmodes = { opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx), opmode((byte)0, (byte)0, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgR, OpMode.iABC), opmode((byte)0, (byte)0, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx), opmode((byte)1, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)1, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)1, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC), opmode((byte)1, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)1, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)0, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx), opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx), opmode((byte)1, (byte)0, OpArgMask.OpArgN, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)0, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC), opmode((byte)0, (byte)0, OpArgMask.OpArgN, OpArgMask.OpArgN, OpMode.iABC), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABx), opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC) };

	public static final int NUM_OPCODES = OpCode.OP_VARARG.getValue();
}