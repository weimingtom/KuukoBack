/*
 ** $Id: lopcodes.c,v 1.37.1.1 2007/12/27 13:02:25 roberto Exp $
 ** See Copyright Notice in lua.h
 */
using System;

namespace kurumi
{
	//using lu_byte = System.Byte;
	//using Instruction = System.UInt32;

	public class LuaOpCodes
	{
		/*===========================================================================
		  We assume that instructions are unsigned numbers.
		  All instructions have an opcode in the first 6 bits.
		  Instructions can have the following fields:
			`A' : 8 bits
			`B' : 9 bits
			`C' : 9 bits
			`Bx' : 18 bits (`B' and `C' together)
			`sBx' : signed Bx

		  A signed argument is represented in excess K; that is, the number
		  value is the unsigned value minus K. K is exactly the maximum value
		  for that argument (so that -max is represented by 0, and +max is
		  represented by 2*max), which is half the maximum for the corresponding
		  unsigned argument.
		===========================================================================*/
	
		public enum OpMode 
		{ 
			/* basic instruction format */
			iABC, 
			iABx, 
			iAsBx
		} 

		/*
		 ** size and position of opcode arguments.
		 */
		public const int SIZE_C	= 9;
		public const int SIZE_B	= 9;
		public const int SIZE_Bx = (SIZE_C + SIZE_B);
		public const int SIZE_A = 8;

		public const int SIZE_OP = 6;

		public const int POS_OP	= 0;
		public const int POS_A = (POS_OP + SIZE_OP);
		public const int POS_C = (POS_A + SIZE_A);
		public const int POS_B = (POS_C + SIZE_C);
		public const int POS_Bx	= POS_C;

		/*
		 ** limits for opcode arguments.
		 ** we use (signed) int to manipulate most arguments,
		 ** so they must fit in LUAI_BITSINT-1 bits (-1 for sign)
		 */
		//#if SIZE_Bx < LUAI_BITSINT-1
		public const int MAXARG_Bx = ((1<<SIZE_Bx)-1);
		public const int MAXARG_sBx = (MAXARG_Bx>>1);         /* `sBx' is signed */
		//#else
		//public const int MAXARG_Bx			= System.Int32.MaxValue;
		//public const int MAXARG_sBx			= System.Int32.MaxValue;
		//#endif

		//public const uint MAXARG_A = (uint)((1 << (int)SIZE_A) -1);
		//public const uint MAXARG_B = (uint)((1 << (int)SIZE_B) -1);
		//public const uint MAXARG_C = (uint)((1 << (int)SIZE_C) -1);
		public const long MAXARG_A = (long)(((1 << (int)SIZE_A) -1) & 0xffffffff);
		public const long MAXARG_B = (long)(((1 << (int)SIZE_B) -1) & 0xffffffff);
		public const long MAXARG_C = (long)(((1 << (int)SIZE_C) -1) & 0xffffffff);

		/* creates a mask with `n' 1 bits at position `p' */
		//public static int MASK1(int n, int p) { return ((~((~(Instruction)0) << n)) << p); }
		public static long/*uint*/ MASK1(int n, int p) 
		{ 
			//return (uint)((~((~0) << n)) << p); 
			return (long)(((~((~0) << n)) << p) & 0xffffffff);
		}

		/* creates a mask with `n' 0 bits at position `p' */
		public static long/*uint*/ MASK0(int n, int p) 
		{ 
			//return (uint)(~MASK1(n, p)); 
			return (long)((~MASK1(n, p)) & 0xffffffff);
		}

		/*
		 ** the following macros help to manipulate instructions
		 */
		public static OpCode GET_OPCODE(long/*UInt32*//*Instruction*/ i)
		{
            return (OpCode)longToOpCode((i >> POS_OP) & MASK1(SIZE_OP, 0));
		}
		
		public static OpCode GET_OPCODE(LuaCode.InstructionPtr i) 
		{ 
			return GET_OPCODE(i.get(0)); 
		}

		public static void SET_OPCODE(/*ref*/ long[]/*UInt32*//*Instruction*/ i, long/*UInt32*//*Instruction*/ o)
		{
		    i[0] = (long/*UInt32*//*Instruction*/)(i[0] & MASK0(SIZE_OP, POS_OP)) | ((o << POS_OP) & MASK1(SIZE_OP, POS_OP));
		}
		
		public static void SET_OPCODE(/*ref*/ long[]/*UInt32*//*Instruction*/ i, OpCode opcode)
		{
		    i[0] = (long/*UInt32*//*Instruction*/)(i[0] & MASK0(SIZE_OP, POS_OP)) | (((long/*uint*/)opcode << POS_OP) & MASK1(SIZE_OP, POS_OP));
		}
		
		public static void SET_OPCODE(LuaCode.InstructionPtr i, OpCode opcode) 
		{ 
			long[] c_ref = new long[1];
			c_ref[0] = i.codes[i.pc];
			SET_OPCODE(/*ref*/ c_ref, opcode);
			i.codes[i.pc] = c_ref[0];
		}

		public static int GETARG_A(long/*UInt32*//*Instruction*/ i)
		{
			return (int)((i >> POS_A) & MASK1(SIZE_A, 0));
		}
		
		public static int GETARG_A(LuaCode.InstructionPtr i) 
		{ 
			return GETARG_A(i.get(0)); 
		}

		public static void SETARG_A(LuaCode.InstructionPtr i, int u)
		{
			i.set(0, (long/*UInt32*//*Instruction*/)((i.get(0) & MASK0(SIZE_A, POS_A)) | ((u << POS_A) & MASK1(SIZE_A, POS_A))));
		}

		public static int GETARG_B(long/*UInt32*//*Instruction*/ i)
		{
			return (int)((i >> POS_B) & MASK1(SIZE_B, 0));
		}
		
		public static int GETARG_B(LuaCode.InstructionPtr i)
		{ 
			return GETARG_B(i.get(0)); 
		}

		public static void SETARG_B(LuaCode.InstructionPtr i, int b)
		{
			i.set(0, (long/*UInt32*//*Instruction*/)((i.get(0) & MASK0(SIZE_B, POS_B)) | ((b << POS_B) & MASK1(SIZE_B, POS_B))));
		}

		public static int GETARG_C(long/*UInt32*//*Instruction*/ i)
		{
			return (int)((i>>POS_C) & MASK1(SIZE_C,0));
		}
		
		public static int GETARG_C(LuaCode.InstructionPtr i)
		{ 
			return GETARG_C(i.get(0)); 
		}

		public static void SETARG_C(LuaCode.InstructionPtr i, int b)
		{
			i.set(0, (long/*UInt32*//*Instruction*/)((i.get(0) & MASK0(SIZE_C, POS_C)) | ((b << POS_C) & MASK1(SIZE_C, POS_C))));
		}

		public static int GETARG_Bx(long/*UInt32*//*Instruction*/ i)
		{
			return (int)((i>>POS_Bx) & MASK1(SIZE_Bx,0));
		}
		
		public static int GETARG_Bx(LuaCode.InstructionPtr i)
		{ 
			return GETARG_Bx(i.get(0)); 
		}

		public static void SETARG_Bx(LuaCode.InstructionPtr i, int b)
		{
			i.set(0, (long/*UInt32*//*Instruction*/)((i.get(0) & MASK0(SIZE_Bx, POS_Bx)) | ((b << POS_Bx) & MASK1(SIZE_Bx, POS_Bx))));
		}

		public static int GETARG_sBx(long/*UInt32*//*Instruction*/ i)
		{
			return (GETARG_Bx(i) - MAXARG_sBx);
		}
		
		public static int GETARG_sBx(LuaCode.InstructionPtr i)
		{ 
			return GETARG_sBx(i.get(0)); 
		}

		public static void SETARG_sBx(LuaCode.InstructionPtr i, int b)
		{
			SETARG_Bx(i, b + MAXARG_sBx);
		}

		//FIXME:long
		public static int CREATE_ABC(OpCode o, int a, int b, int c)
		{
			return (int)(((int)o << POS_OP) | (a << POS_A) | (b << POS_B) | (c << POS_C));
		}

		//FIXME:long
		public static int CREATE_ABx(OpCode o, int a, int bc)
		{
			int result = (int)(((int)o << POS_OP) | (a << POS_A) | (bc << POS_Bx));
			return (int)(((int)o << POS_OP) | (a << POS_A) | (bc << POS_Bx));
		}

		/*
		 ** Macros to operate RK indices
		 */
		
		/* this bit 1 means constant (0 means register) */
		public readonly static int BITRK = (1 << (SIZE_B - 1));

		/* test whether value is a constant */
		public static int ISK(int x)		
		{
			return x & BITRK;
		}

		/* gets the index of the constant */
		public static int INDEXK(int r)	
		{
			return r & (~BITRK);
		}

		public static readonly int MAXINDEXRK = BITRK - 1;

		/* code a constant index as a RK value */
		public static int RKASK(int x)	
		{
			return x | BITRK;
		}

		/*
		 ** invalid register that fits in 8 bits
		 */
		public static readonly int NO_REG = (int)MAXARG_A;

		/*
		 ** R(x) - register
		 ** Kst(x) - constant (in constant table)
		 ** RK(x) == if ISK(x) then Kst(INDEXK(x)) else R(x)
		 */

		
		
		public enum OpCode 
		{
			/*----------------------------------------------------------------------
			name		args	description
			------------------------------------------------------------------------*/
			OP_MOVE,/*	A B	R(A) := R(B)					*/
			OP_LOADK,/*	A Bx	R(A) := Kst(Bx)					*/
			OP_LOADBOOL,/*	A B C	R(A) := (Bool)B; if (C) pc++			*/
			OP_LOADNIL,/*	A B	R(A) := ... := R(B) := nil			*/
			OP_GETUPVAL,/*	A B	R(A) := UpValue[B]				*/
			
			OP_GETGLOBAL,/*	A Bx	R(A) := Gbl[Kst(Bx)]				*/
			OP_GETTABLE,/*	A B C	R(A) := R(B)[RK(C)]				*/
			
			OP_SETGLOBAL,/*	A Bx	Gbl[Kst(Bx)] := R(A)				*/
			OP_SETUPVAL,/*	A B	UpValue[B] := R(A)				*/
			OP_SETTABLE,/*	A B C	R(A)[RK(B)] := RK(C)				*/
			
			OP_NEWTABLE,/*	A B C	R(A) := {} (size = B,C)				*/
			
			OP_SELF,/*	A B C	R(A+1) := R(B); R(A) := R(B)[RK(C)]		*/
			
			OP_ADD,/*	A B C	R(A) := RK(B) + RK(C)				*/
			OP_SUB,/*	A B C	R(A) := RK(B) - RK(C)				*/
			OP_MUL,/*	A B C	R(A) := RK(B) * RK(C)				*/
			OP_DIV,/*	A B C	R(A) := RK(B) / RK(C)				*/
			OP_MOD,/*	A B C	R(A) := RK(B) % RK(C)				*/
			OP_POW,/*	A B C	R(A) := RK(B) ^ RK(C)				*/
			OP_UNM,/*	A B	R(A) := -R(B)					*/
			OP_NOT,/*	A B	R(A) := not R(B)				*/
			OP_LEN,/*	A B	R(A) := length of R(B)				*/
			
			OP_CONCAT,/*	A B C	R(A) := R(B).. ... ..R(C)			*/
			
			OP_JMP,/*	sBx	pc+=sBx					*/
			
			OP_EQ,/*	A B C	if ((RK(B) == RK(C)) ~= A) then pc++		*/
			OP_LT,/*	A B C	if ((RK(B) <  RK(C)) ~= A) then pc++  		*/
			OP_LE,/*	A B C	if ((RK(B) <= RK(C)) ~= A) then pc++  		*/
			
			OP_TEST,/*	A C	if not (R(A) <=> C) then pc++			*/
			OP_TESTSET,/*	A B C	if (R(B) <=> C) then R(A) := R(B) else pc++	*/
			
			OP_CALL,/*	A B C	R(A), ... ,R(A+C-2) := R(A)(R(A+1), ... ,R(A+B-1)) */
			OP_TAILCALL,/*	A B C	return R(A)(R(A+1), ... ,R(A+B-1))		*/
			OP_RETURN,/*	A B	return R(A), ... ,R(A+B-2)	(see note)	*/
			
			OP_FORLOOP,/*	A sBx	R(A)+=R(A+2);
						if R(A) <?= R(A+1) then { pc+=sBx; R(A+3)=R(A) }*/
			OP_FORPREP,/*	A sBx	R(A)-=R(A+2); pc+=sBx				*/
			
			OP_TFORLOOP,/*	A C	R(A+3), ... ,R(A+2+C) := R(A)(R(A+1), R(A+2));
									if R(A+3) ~= nil then R(A+2)=R(A+3) else pc++	*/
			OP_SETLIST,/*	A B C	R(A)[(C-1)*FPF+i] := R(A+i), 1 <= i <= B	*/
			
			OP_CLOSE,/*	A 	close all variables in the stack up to (>=) R(A)*/
			OP_CLOSURE,/*	A Bx	R(A) := closure(KPROTO[Bx], R(A), ... ,R(A+n))	*/
			
			OP_VARARG/*	A B	R(A), R(A+1), ..., R(A+B-1) = vararg		*/
		}
		
        public static long opCodeToLong(LuaOpCodes.OpCode code)
        {
            switch (code)
            {
                case LuaOpCodes.OpCode.OP_MOVE:
                    return 0;
                case LuaOpCodes.OpCode.OP_LOADK:
                    return 1;
                case LuaOpCodes.OpCode.OP_LOADBOOL:
                    return 2;
                case LuaOpCodes.OpCode.OP_LOADNIL:
                    return 3;
		        case LuaOpCodes.OpCode.OP_GETUPVAL:
                    return 4;
		        case LuaOpCodes.OpCode.OP_GETGLOBAL:
                    return 5;
		        case LuaOpCodes.OpCode.OP_GETTABLE:
                    return 6;
		        case LuaOpCodes.OpCode.OP_SETGLOBAL:
                    return 7;
		        case LuaOpCodes.OpCode.OP_SETUPVAL:
                    return 8;
		        case LuaOpCodes.OpCode.OP_SETTABLE:
                    return 9;
		        case LuaOpCodes.OpCode.OP_NEWTABLE:
                    return 10;
		        case LuaOpCodes.OpCode.OP_SELF:
                    return 11;
		        case LuaOpCodes.OpCode.OP_ADD:
                    return 12;
		        case LuaOpCodes.OpCode.OP_SUB:
                    return 13;
		        case LuaOpCodes.OpCode.OP_MUL:
                    return 14;
		        case LuaOpCodes.OpCode.OP_DIV:
                    return 15;
		        case LuaOpCodes.OpCode.OP_MOD:
                    return 16;
		        case LuaOpCodes.OpCode.OP_POW:
                    return 17;
		        case LuaOpCodes.OpCode.OP_UNM:
                    return 18;
		        case LuaOpCodes.OpCode.OP_NOT:
                    return 19;
		        case LuaOpCodes.OpCode.OP_LEN:
                    return 20;
		        case LuaOpCodes.OpCode.OP_CONCAT:
                    return 21;
		        case LuaOpCodes.OpCode.OP_JMP:
                    return 22;
		        case LuaOpCodes.OpCode.OP_EQ:
                    return 23;
		        case LuaOpCodes.OpCode.OP_LT:
                    return 24;
		        case LuaOpCodes.OpCode.OP_LE:
                    return 25;
		        case LuaOpCodes.OpCode.OP_TEST:
                    return 26;
		        case LuaOpCodes.OpCode.OP_TESTSET:
                    return 27;
		        case LuaOpCodes.OpCode.OP_CALL:
                    return 28;
		        case LuaOpCodes.OpCode.OP_TAILCALL:
                    return 29;
		        case LuaOpCodes.OpCode.OP_RETURN:
                    return 30;
                case LuaOpCodes.OpCode.OP_FORLOOP:
                    return 31;
                case LuaOpCodes.OpCode.OP_FORPREP:
                    return 32;
                case LuaOpCodes.OpCode.OP_TFORLOOP:
                    return 33;
                case LuaOpCodes.OpCode.OP_SETLIST:
                    return 34;
                case LuaOpCodes.OpCode.OP_CLOSE:
                    return 35;
                case LuaOpCodes.OpCode.OP_CLOSURE:
                    return 36;
                case LuaOpCodes.OpCode.OP_VARARG:
                    return 37;
            }
            throw new Exception("OpCode error");
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
            throw new Exception("OpCode error");
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
		
		public enum OpArgMask 
		{
			OpArgN,  /* argument is not used */
			OpArgU,  /* argument is used */
			OpArgR,  /* argument is a register or a jump offset */
			OpArgK   /* argument is a constant or register/constant */
		}

		/*
		 ** grep "ORDER OP" if you change these enums
		 */
		public static OpMode getOpMode(OpCode m)	
		{
            switch (luaP_opmodes[(int)m] & 3)
            {
                default:
                case 0:
                    return OpMode.iABC;
                case 1:
                    return OpMode.iABx;
                case 2:
                    return OpMode.iAsBx;
            }
		}
		
		public static OpArgMask getBMode(OpCode m) 
		{
            switch ((luaP_opmodes[(int)m] >> 4) & 3)
            {
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
		
		public static OpArgMask getCMode(OpCode m)
		{
            switch ((luaP_opmodes[(int)m] >> 2) & 3)
            {
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
		
		public static int testAMode(OpCode m) 
		{ 
			return luaP_opmodes[(int)m] & (1 << 6); 
		}
		
		public static int testTMode(OpCode m)
		{ 
			return luaP_opmodes[(int)m] & (1 << 7); 
		}

		/* number of list items to accumulate before a SETLIST instruction */
		public const int LFIELDS_PER_FLUSH = 50;

		/* ORDER OP */
		public/*private*/ readonly static CLib.CharPtr[] luaP_opnames = {
			CLib.CharPtr.toCharPtr("MOVE"),
			CLib.CharPtr.toCharPtr("LOADK"),
			CLib.CharPtr.toCharPtr("LOADBOOL"),
			CLib.CharPtr.toCharPtr("LOADNIL"),
			CLib.CharPtr.toCharPtr("GETUPVAL"),
			CLib.CharPtr.toCharPtr("GETGLOBAL"),
			CLib.CharPtr.toCharPtr("GETTABLE"),
			CLib.CharPtr.toCharPtr("SETGLOBAL"),
			CLib.CharPtr.toCharPtr("SETUPVAL"),
			CLib.CharPtr.toCharPtr("SETTABLE"),
			CLib.CharPtr.toCharPtr("NEWTABLE"),
			CLib.CharPtr.toCharPtr("SELF"),
			CLib.CharPtr.toCharPtr("ADD"),
			CLib.CharPtr.toCharPtr("SUB"),
			CLib.CharPtr.toCharPtr("MUL"),
			CLib.CharPtr.toCharPtr("DIV"),
			CLib.CharPtr.toCharPtr("MOD"),
			CLib.CharPtr.toCharPtr("POW"),
			CLib.CharPtr.toCharPtr("UNM"),
			CLib.CharPtr.toCharPtr("NOT"),
			CLib.CharPtr.toCharPtr("LEN"),
			CLib.CharPtr.toCharPtr("CONCAT"),
			CLib.CharPtr.toCharPtr("JMP"),
			CLib.CharPtr.toCharPtr("EQ"),
			CLib.CharPtr.toCharPtr("LT"),
			CLib.CharPtr.toCharPtr("LE"),
			CLib.CharPtr.toCharPtr("TEST"),
			CLib.CharPtr.toCharPtr("TESTSET"),
			CLib.CharPtr.toCharPtr("CALL"),
			CLib.CharPtr.toCharPtr("TAILCALL"),
			CLib.CharPtr.toCharPtr("RETURN"),
			CLib.CharPtr.toCharPtr("FORLOOP"),
			CLib.CharPtr.toCharPtr("FORPREP"),
			CLib.CharPtr.toCharPtr("TFORLOOP"),
			CLib.CharPtr.toCharPtr("SETLIST"),
			CLib.CharPtr.toCharPtr("CLOSE"),
			CLib.CharPtr.toCharPtr("CLOSURE"),
			CLib.CharPtr.toCharPtr("VARARG"),
		};


		private static Byte/*lu_byte*/ opmode(Byte/*lu_byte*/ t, Byte/*lu_byte*/ a, OpArgMask b, OpArgMask c, OpMode m)
		{
            int bValue = 0;
            int cValue = 0;
            int mValue = 0;
            switch (b)
            {
                case OpArgMask.OpArgN:
                    bValue = 0;
                    break;
                case OpArgMask.OpArgU:
                    bValue = 1;
                    break;
                case OpArgMask.OpArgR:
                    bValue = 2;
                    break;
                case OpArgMask.OpArgK:
                    bValue = 3;
                    break;
            }
            switch (c)
            {
                case OpArgMask.OpArgN:
                    cValue = 0;
                    break;
                case OpArgMask.OpArgU:
                    cValue = 1;
                    break;
                case OpArgMask.OpArgR:
                    cValue = 2;
                    break;
                case OpArgMask.OpArgK:
                    cValue = 3;
                    break;
            }
            switch (m)
            {
                case OpMode.iABC:
                    mValue = 0;
                    break;
                case OpMode.iABx:
                    mValue = 1;
                    break;
                case OpMode.iAsBx:
                    mValue = 2;
                    break;
            }
            return (Byte/*lu_byte*/)(((t) << 7) | ((a) << 6) | (((Byte/*lu_byte*/)bValue) << 4) | (((Byte/*lu_byte*/)cValue) << 2) | ((Byte/*lu_byte*/)mValue));
		}
		
		//       T  A    B       C     mode		   opcode		
		//lu_byte[]
		private readonly static Byte[] luaP_opmodes = {
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx),
			opmode((byte)0, (byte)0, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgR, OpMode.iABC),
			opmode((byte)0, (byte)0, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx),
			opmode((byte)1, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)1, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)1, (byte)0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC),
			opmode((byte)1, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)1, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)0, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx),
			opmode((byte)0, (byte)1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx),
			opmode((byte)1, (byte)0, OpArgMask.OpArgN, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)0, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC),
			opmode((byte)0, (byte)0, OpArgMask.OpArgN, OpArgMask.OpArgN, OpMode.iABC),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABx),
			opmode((byte)0, (byte)1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC)
		};

		public const int NUM_OPCODES = (int)OpCode.OP_VARARG;
	}
}
