/*
 ** $Id: lcode.c,v 2.25.1.3 2007/12/28 15:32:23 roberto Exp $
 ** Code generator for Lua
 ** See Copyright Notice in lua.h
 */
using System;

namespace kurumi
{
	//using TValue = Lua.TValue;
	//using lua_Number = System.Double;
	//using Instruction = System.UInt32;

	public class LuaCode
	{
		public class InstructionPtr
		{
			public long[]/*UInt32[]*//*Instruction[]*/ codes;
			public int pc;
			
			public InstructionPtr()
			{
				this.codes = null;
				this.pc = -1;
			}
			
			public InstructionPtr(long[]/*UInt32[]*//*Instruction[]*/ codes, int pc)
			{
				this.codes = codes;
				this.pc = pc;
			}
			
			public static InstructionPtr Assign(InstructionPtr ptr)
			{
				if (ptr == null)
				{
					return null;
				}
				return new InstructionPtr(ptr.codes, ptr.pc);
			}
	
	        //UInt32/*Instruction*/ this[int index]
	        public long/*UInt32*//*Instruction*/ get(int index)
	        {
	                return this.codes[pc + index];
	        }
	        public void set(int index, long/*UInt32*//*Instruction*/ val)
	        {
	            this.codes[pc + index] = val;
	        }
	
	        public static InstructionPtr inc(/*ref*/ InstructionPtr[] ptr)
			{
	        	InstructionPtr result = new InstructionPtr(ptr[0].codes, ptr[0].pc);
				ptr[0].pc++;
				return result;
			}
			
	        public static InstructionPtr dec(/*ref*/ InstructionPtr[] ptr)
			{
	        	InstructionPtr result = new InstructionPtr(ptr[0].codes, ptr[0].pc);
				ptr[0].pc--;
				return result;
			}
	
	        //operator <
			public static bool lessThan(InstructionPtr p1, InstructionPtr p2)
			{
	            ClassType.Assert(p1.codes == p2.codes);
				return p1.pc < p2.pc;
			}
	
	        //operator >
			public static bool greaterThan(InstructionPtr p1, InstructionPtr p2)
			{
	            ClassType.Assert(p1.codes == p2.codes);
				return p1.pc > p2.pc;
			}
	
	        //operator <=
			public static bool lessEqual(InstructionPtr p1, InstructionPtr p2)
			{
	            ClassType.Assert(p1.codes == p2.codes);
				return p1.pc < p2.pc;
			}
	
	        //operator >=
			public static bool greaterEqual(InstructionPtr p1, InstructionPtr p2)
			{
	            ClassType.Assert(p1.codes == p2.codes);
				return p1.pc > p2.pc;
			}
		}		
		
		/*
		 ** Marks the end of a patch list. It is an invalid value both as an absolute
		 ** address, and as a list link (would link an element to itself).
		 */
		public const int NO_JUMP = (-1);

		/*
		 ** grep "ORDER OPR" if you change these enums
		 */
		public enum BinOpr
		{
			OPR_ADD, 
			OPR_SUB, 
			OPR_MUL, 
			OPR_DIV, 
			OPR_MOD, 
			OPR_POW,
			OPR_CONCAT,
			OPR_NE, 
			OPR_EQ,
			OPR_LT, 
			OPR_LE, 
			OPR_GT, 
			OPR_GE,
			OPR_AND, 
			OPR_OR,
			OPR_NOBINOPR
		}
		
		public enum UnOpr 
		{ 
			OPR_MINUS, 
			OPR_NOT, 
			OPR_LEN, 
			OPR_NOUNOPR
		}
		
		public static InstructionPtr getcode(LuaParser.FuncState fs, LuaParser.expdesc e)	
		{
			return new InstructionPtr(fs.f.code, e.u.s.info);
		}

		public static int luaK_codeAsBx(LuaParser.FuncState fs, LuaOpCodes.OpCode o, int A, int sBx) 
		{ 
			return LuaCode.luaK_codeABx(fs, o, A, sBx + LuaOpCodes.MAXARG_sBx); 
		}

		public static void luaK_setmultret(LuaParser.FuncState fs, LuaParser.expdesc e) 
		{ 
			LuaCode.luaK_setreturns(fs, e, Lua.LUA_MULTRET); 
		}

		public static bool hasjumps(LuaParser.expdesc e) 
		{ 
			return e.t != e.f; 
		}

		private static int isnumeral(LuaParser.expdesc e)
		{
			return (e.k == LuaParser.expkind.VKNUM && e.t == NO_JUMP && e.f == NO_JUMP) ? 1 : 0;
		}

		public static void luaK_nil(LuaParser.FuncState fs, int from, int n)
		{
			InstructionPtr previous;
			if (fs.pc > fs.lasttarget) 
			{  
				/* no jumps to current position? */
				if (fs.pc == 0) 
				{  
					/* function start? */
					if (from >= fs.nactvar)
					{
						return;  /* positions are already clean */
					}
				}
				else 
				{
					previous = new InstructionPtr(fs.f.code, fs.pc-1);
					if (LuaOpCodes.GET_OPCODE(previous) == LuaOpCodes.OpCode.OP_LOADNIL)
					{
						int pfrom = LuaOpCodes.GETARG_A(previous);
						int pto = LuaOpCodes.GETARG_B(previous);
						if (pfrom <= from && from <= pto+1) 
						{  
							/* can connect both? */
							if (from+n-1 > pto)
							{
								LuaOpCodes.SETARG_B(previous, from + n - 1);
							}
							return;
						}
					}
				}
			}
			luaK_codeABC(fs, LuaOpCodes.OpCode.OP_LOADNIL, from, from + n - 1, 0);  /* else no optimization */
		}

		public static int luaK_jump(LuaParser.FuncState fs)
		{
			int jpc = fs.jpc;  /* save list of jumps to here */
			int[] j = new int[1];
			j[0] = 0;
			fs.jpc = NO_JUMP;
			j[0] = luaK_codeAsBx(fs, LuaOpCodes.OpCode.OP_JMP, 0, NO_JUMP);
			luaK_concat(fs, /*ref*/ j, jpc);  /* keep them on hold */
			return j[0];
		}

		public static void luaK_ret(LuaParser.FuncState fs, int first, int nret)
		{
			luaK_codeABC(fs, LuaOpCodes.OpCode.OP_RETURN, first, nret + 1, 0);
		}

		private static int condjump(LuaParser.FuncState fs, LuaOpCodes.OpCode op, int A, int B, int C)
		{
			luaK_codeABC(fs, op, A, B, C);
			return luaK_jump(fs);
		}

		private static void fixjump(LuaParser.FuncState fs, int pc, int dest)
		{
			InstructionPtr jmp = new InstructionPtr(fs.f.code, pc);
			int offset = dest-(pc+1);
			LuaLimits.lua_assert(dest != NO_JUMP);
			if (Math.Abs(offset) > LuaOpCodes.MAXARG_sBx)
			{
				LuaLex.luaX_syntaxerror(fs.ls, CLib.CharPtr.toCharPtr("control structure too long"));
			}
			LuaOpCodes.SETARG_sBx(jmp, offset);
		}

		/*
		 ** returns current `pc' and marks it as a jump target (to avoid wrong
		 ** optimizations with consecutive instructions not in the same basic block).
		 */
		public static int luaK_getlabel(LuaParser.FuncState fs)
		{
			fs.lasttarget = fs.pc;
			return fs.pc;
		}

		private static int getjump(LuaParser.FuncState fs, int pc)
		{
			int offset = LuaOpCodes.GETARG_sBx(fs.f.code[pc]);
			if (offset == NO_JUMP)  /* point to itself represents end of list */
			{
				return NO_JUMP;  /* end of list */
			}
			else
			{
				return (pc+1)+offset;  /* turn offset into absolute position */
			}
		}

		private static InstructionPtr getjumpcontrol(LuaParser.FuncState fs, int pc)
		{
			InstructionPtr pi = new InstructionPtr(fs.f.code, pc);
			if (pc >= 1 && (LuaOpCodes.testTMode(LuaOpCodes.GET_OPCODE(pi.get(-1))) != 0))
			{
				return new InstructionPtr(pi.codes, pi.pc-1);
			}
			else
			{
				return new InstructionPtr(pi.codes, pi.pc);
			}
		}

		/*
		 ** check whether list has any jump that do not produce a value
		 ** (or produce an inverted value)
		 */
		private static int need_value(LuaParser.FuncState fs, int list)
		{
			for (; list != NO_JUMP; list = getjump(fs, list)) 
			{
				InstructionPtr i = getjumpcontrol(fs, list);
				if (LuaOpCodes.GET_OPCODE(i.get(0)) != LuaOpCodes.OpCode.OP_TESTSET) 
				{
					return 1;
				}
			}
			return 0;  /* not found */
		}

		private static int patchtestreg(LuaParser.FuncState fs, int node, int reg)
		{
			InstructionPtr i = getjumpcontrol(fs, node);
			if (LuaOpCodes.GET_OPCODE(i.get(0)) != LuaOpCodes.OpCode.OP_TESTSET)
			{
				return 0;  /* cannot patch other instructions */
			}
			if (reg != LuaOpCodes.NO_REG && reg != LuaOpCodes.GETARG_B(i.get(0)))
			{
				LuaOpCodes.SETARG_A(i, reg);
			}
			else  /* no register to put value or register already has the value */
			{
				i.set(0, (/*uint*/long)(LuaOpCodes.CREATE_ABC(LuaOpCodes.OpCode.OP_TEST, LuaOpCodes.GETARG_B(i.get(0)), 0, LuaOpCodes.GETARG_C(i.get(0))) &0xffffffff) );
			}
			
			return 1;
		}

		private static void removevalues(LuaParser.FuncState fs, int list)
		{
			for (; list != NO_JUMP; list = getjump(fs, list))
			{
				patchtestreg(fs, list, LuaOpCodes.NO_REG);
			}
		}

		private static void patchlistaux(LuaParser.FuncState fs, int list, int vtarget, int reg, int dtarget) 
		{
			while (list != NO_JUMP) 
			{
				int next = getjump(fs, list);
				if (patchtestreg(fs, list, reg) != 0)
				{
					fixjump(fs, list, vtarget);
				}
				else
				{
					fixjump(fs, list, dtarget);  /* jump to default target */
				}
				list = next;
			}
		}

		private static void dischargejpc(LuaParser.FuncState fs)
		{
			patchlistaux(fs, fs.jpc, fs.pc, LuaOpCodes.NO_REG, fs.pc);
			fs.jpc = NO_JUMP;
		}

		public static void luaK_patchlist(LuaParser.FuncState fs, int list, int target) 
		{
			if (target == fs.pc)
			{
				luaK_patchtohere(fs, list);
			}
			else 
			{
				LuaLimits.lua_assert(target < fs.pc);
				patchlistaux(fs, list, target, LuaOpCodes.NO_REG, target);
			}
		}

		public static void luaK_patchtohere(LuaParser.FuncState fs, int list)
		{
			luaK_getlabel(fs);
			int[] jpc_ref = new int[1];
			jpc_ref[0] = fs.jpc;
			luaK_concat(fs, /*ref*/ jpc_ref, list);
			fs.jpc = jpc_ref[0];
		}

		public static void luaK_concat(LuaParser.FuncState fs, /*ref*/ int[] l1, int l2)
		{
			if (l2 == NO_JUMP) 
			{
				return;
			}
			else if (l1[0] == NO_JUMP)
			{
				l1[0] = l2;
			}
			else 
			{
				int list = l1[0];
				int next;
				while ((next = getjump(fs, list)) != NO_JUMP)  /* find last element */
				{
					list = next;
				}
				fixjump(fs, list, l2);
			}
		}

		public static void luaK_checkstack(LuaParser.FuncState fs, int n)
		{
			int newstack = fs.freereg + n;
			if (newstack > fs.f.maxstacksize) 
			{
				if (newstack >= LuaLimits.MAXSTACK)
				{
					LuaLex.luaX_syntaxerror(fs.ls, CLib.CharPtr.toCharPtr("function or expression too complex"));
				}
				fs.f.maxstacksize = LuaLimits.cast_byte(newstack);
			}
		}

		public static void luaK_reserveregs(LuaParser.FuncState fs, int n)
		{
			luaK_checkstack(fs, n);
			fs.freereg += n;
		}

		private static void freereg(LuaParser.FuncState fs, int reg)
		{
			if ((LuaOpCodes.ISK(reg) == 0) && reg >= fs.nactvar)
			{
				fs.freereg--;
				LuaLimits.lua_assert(reg == fs.freereg);
			}
		}

		private static void freeexp(LuaParser.FuncState fs, LuaParser.expdesc e)
		{
			if (e.k == LuaParser.expkind.VNONRELOC)
			{
				freereg(fs, e.u.s.info);
			}
		}

		private static int addk(LuaParser.FuncState fs, LuaObject.TValue k, LuaObject.TValue v)
		{
			LuaState.lua_State L = fs.L;
			LuaObject.TValue idx = LuaTable.luaH_set(L, fs.h, k);
			LuaObject.Proto f = fs.f;
			int oldsize = f.sizek;
			if (LuaObject.ttisnumber(idx))
			{
				LuaLimits.lua_assert(LuaObject.luaO_rawequalObj(fs.f.k[LuaLimits.cast_int(LuaObject.nvalue(idx))], v));
				return LuaLimits.cast_int(LuaObject.nvalue(idx));
			}
			else 
			{  
				/* constant not found; create a new entry */
				LuaObject.setnvalue(idx, LuaLimits.cast_num(fs.nk));
				LuaObject.TValue[][] k_ref = new LuaObject.TValue[1][];
				k_ref[0] = f.k;
				int[] sizek_ref = new int[1];
				sizek_ref[0] = f.sizek;
				LuaMem.luaM_growvector_TValue(L, /*ref*/ k_ref, fs.nk, /*ref*/ sizek_ref,
					LuaOpCodes.MAXARG_Bx, CLib.CharPtr.toCharPtr("constant table overflow"), new ClassType(ClassType.TYPE_TVALUE));
				f.sizek = sizek_ref[0];
				f.k = k_ref[0];
				while (oldsize < f.sizek)
				{
					LuaObject.setnilvalue(f.k[oldsize++]);
				}
				LuaObject.setobj(L, f.k[fs.nk], v);
				LuaGC.luaC_barrier(L, f, v);
				return fs.nk++;
			}
		}

		public static int luaK_stringK(LuaParser.FuncState fs, LuaObject.TString s)
		{
			LuaObject.TValue o = new LuaObject.TValue();
			LuaObject.setsvalue(fs.L, o, s);
			return addk(fs, o, o);
		}

		public static int luaK_numberK(LuaParser.FuncState fs, Double/*lua_Number*/ r)
		{
			LuaObject.TValue o = new LuaObject.TValue();
			LuaObject.setnvalue(o, r);
			return addk(fs, o, o);
		}

		private static int boolK(LuaParser.FuncState fs, int b)
		{
			LuaObject.TValue o = new LuaObject.TValue();
			LuaObject.setbvalue(o, b);
			return addk(fs, o, o);
		}

		private static int nilK(LuaParser.FuncState fs)
		{
			LuaObject.TValue k = new LuaObject.TValue(), v = new LuaObject.TValue();
			LuaObject.setnilvalue(v);
			/* cannot use nil as key; instead use table itself to represent nil */
			LuaObject.sethvalue(fs.L, k, fs.h);
			return addk(fs, k, v);
		}

		public static void luaK_setreturns(LuaParser.FuncState fs, LuaParser.expdesc e, int nresults)
		{
			if (e.k == LuaParser.expkind.VCALL)
			{  
				/* expression is an open function call? */
				LuaOpCodes.SETARG_C(getcode(fs, e), nresults + 1);
			}
			else if (e.k == LuaParser.expkind.VVARARG)
			{
				LuaOpCodes.SETARG_B(getcode(fs, e), nresults + 1);
				LuaOpCodes.SETARG_A(getcode(fs, e), fs.freereg);
				luaK_reserveregs(fs, 1);
			}
		}

		public static void luaK_setoneret(LuaParser.FuncState fs, LuaParser.expdesc e)
		{
			if (e.k == LuaParser.expkind.VCALL)
			{  
				/* expression is an open function call? */
				e.k = LuaParser.expkind.VNONRELOC;
				e.u.s.info = LuaOpCodes.GETARG_A(getcode(fs, e));
			}
			else if (e.k == LuaParser.expkind.VVARARG)
			{
				LuaOpCodes.SETARG_B(getcode(fs, e), 2);
				e.k = LuaParser.expkind.VRELOCABLE;  /* can relocate its simple result */
			}
		}

		public static void luaK_dischargevars(LuaParser.FuncState fs, LuaParser.expdesc e)
		{
			switch (e.k) 
			{
				case LuaParser.expkind.VLOCAL:
					{
						e.k = LuaParser.expkind.VNONRELOC;
						break;
					}
				case LuaParser.expkind.VUPVAL:
					{
						e.u.s.info = luaK_codeABC(fs, LuaOpCodes.OpCode.OP_GETUPVAL, 0, e.u.s.info, 0);
						e.k = LuaParser.expkind.VRELOCABLE;
						break;
					}
				case LuaParser.expkind.VGLOBAL:
					{
						e.u.s.info = luaK_codeABx(fs, LuaOpCodes.OpCode.OP_GETGLOBAL, 0, e.u.s.info);
						e.k = LuaParser.expkind.VRELOCABLE;
						break;
					}
				case LuaParser.expkind.VINDEXED:
					{
						freereg(fs, e.u.s.aux);
						freereg(fs, e.u.s.info);
						e.u.s.info = luaK_codeABC(fs, LuaOpCodes.OpCode.OP_GETTABLE, 0, e.u.s.info, e.u.s.aux);
						e.k = LuaParser.expkind.VRELOCABLE;
						break;
					}
				case LuaParser.expkind.VVARARG:
				case LuaParser.expkind.VCALL:
					{
						luaK_setoneret(fs, e);
						break;
					}
				default: 
					{
						break;  /* there is one value available (somewhere) */
					}
			}
		}

		private static int code_label(LuaParser.FuncState fs, int A, int b, int jump)
		{
			luaK_getlabel(fs);  /* those instructions may be jump targets */
			return luaK_codeABC(fs, LuaOpCodes.OpCode.OP_LOADBOOL, A, b, jump);
		}

		private static void discharge2reg(LuaParser.FuncState fs, LuaParser.expdesc e, int reg)
		{
			luaK_dischargevars(fs, e);
			switch (e.k) 
			{
				case LuaParser.expkind.VNIL: 
					{
						luaK_nil(fs, reg, 1);
						break;
					}
				case LuaParser.expkind.VFALSE:  
				case LuaParser.expkind.VTRUE: 
					{
						luaK_codeABC(fs, LuaOpCodes.OpCode.OP_LOADBOOL, reg, (e.k == LuaParser.expkind.VTRUE) ? 1 : 0, 0);
						break;
					}
				case LuaParser.expkind.VK: 
					{
						luaK_codeABx(fs, LuaOpCodes.OpCode.OP_LOADK, reg, e.u.s.info);
						break;
					}
				case LuaParser.expkind.VKNUM: 
					{
						luaK_codeABx(fs, LuaOpCodes.OpCode.OP_LOADK, reg, luaK_numberK(fs, e.u.nval));
						break;
					}
				case LuaParser.expkind.VRELOCABLE: 
					{
						InstructionPtr pc = getcode(fs, e);
						LuaOpCodes.SETARG_A(pc, reg);
						break;
					}
				case LuaParser.expkind.VNONRELOC: 
					{
						if (reg != e.u.s.info)
						{
							luaK_codeABC(fs, LuaOpCodes.OpCode.OP_MOVE, reg, e.u.s.info, 0);
						}
						break;
					}
				default: 
					{
						LuaLimits.lua_assert(e.k == LuaParser.expkind.VVOID || e.k == LuaParser.expkind.VJMP);
						return;  /* nothing to do... */
					}
			}
			e.u.s.info = reg;
			e.k = LuaParser.expkind.VNONRELOC;
		}

		private static void discharge2anyreg(LuaParser.FuncState fs, LuaParser.expdesc e)
		{
			if (e.k != LuaParser.expkind.VNONRELOC)
			{
				luaK_reserveregs(fs, 1);
				discharge2reg(fs, e, fs.freereg-1);
			}
		}

		private static void exp2reg(LuaParser.FuncState fs, LuaParser.expdesc e, int reg)
		{
			discharge2reg(fs, e, reg);
			if (e.k == LuaParser.expkind.VJMP)
			{
				int[] t_ref = new int[1];
				t_ref[0] = e.t;
				luaK_concat(fs, /*ref*/ t_ref, e.u.s.info);  /* put this jump in `t' list */
				e.t = t_ref[0];
			}
			if (hasjumps(e)) 
			{
				int final_;  /* position after whole expression */
				int p_f = NO_JUMP;  /* position of an eventual LOAD false */
				int p_t = NO_JUMP;  /* position of an eventual LOAD true */
				if (need_value(fs, e.t) != 0 || need_value(fs, e.f) != 0) 
				{
					int fj = (e.k == LuaParser.expkind.VJMP) ? NO_JUMP : luaK_jump(fs);
					p_f = code_label(fs, reg, 0, 1);
					p_t = code_label(fs, reg, 1, 0);
					luaK_patchtohere(fs, fj);
				}
				final_ = luaK_getlabel(fs);
				patchlistaux(fs, e.f, final_, reg, p_f);
				patchlistaux(fs, e.t, final_, reg, p_t);
			}
			e.f = e.t = NO_JUMP;
			e.u.s.info = reg;
			e.k = LuaParser.expkind.VNONRELOC;
		}

		public static void luaK_exp2nextreg(LuaParser.FuncState fs, LuaParser.expdesc e)
		{
			luaK_dischargevars(fs, e);
			freeexp(fs, e);
			luaK_reserveregs(fs, 1);
			exp2reg(fs, e, fs.freereg - 1);
		}

		public static int luaK_exp2anyreg(LuaParser.FuncState fs, LuaParser.expdesc e)
		{
			luaK_dischargevars(fs, e);
			if (e.k == LuaParser.expkind.VNONRELOC) 
			{
				if (!hasjumps(e)) 
				{
					return e.u.s.info;  /* exp is already in a register */
				}
				if (e.u.s.info >= fs.nactvar) 
				{  
					/* reg. is not a local? */
					exp2reg(fs, e, e.u.s.info);  /* put value on it */
					return e.u.s.info;
				}
			}
			luaK_exp2nextreg(fs, e);  /* default */
			return e.u.s.info;
		}

		public static void luaK_exp2val(LuaParser.FuncState fs, LuaParser.expdesc e)
		{
			if (hasjumps(e))
			{
				luaK_exp2anyreg(fs, e);
			}
			else
			{
				luaK_dischargevars(fs, e);
			}
		}

		public static int luaK_exp2RK(LuaParser.FuncState fs, LuaParser.expdesc e)
		{
			luaK_exp2val(fs, e);
			switch (e.k) 
			{
				case LuaParser.expkind.VKNUM:
				case LuaParser.expkind.VTRUE:
				case LuaParser.expkind.VFALSE:
				case LuaParser.expkind.VNIL: 
					{
						if (fs.nk <= LuaOpCodes.MAXINDEXRK)
						{  
							/* constant fit in RK operand? */
							e.u.s.info = (e.k == LuaParser.expkind.VNIL)  ? nilK(fs) :
								(e.k == LuaParser.expkind.VKNUM) ? luaK_numberK(fs, e.u.nval) :
								boolK(fs, (e.k == LuaParser.expkind.VTRUE) ? 1 : 0);
							e.k = LuaParser.expkind.VK;
							return LuaOpCodes.RKASK(e.u.s.info);
						}
						else 
						{
							break;
						}
					}
				case LuaParser.expkind.VK: 
					{
						if (e.u.s.info <= LuaOpCodes.MAXINDEXRK)  /* constant fit in argC? */
						{
							return LuaOpCodes.RKASK(e.u.s.info);
						}
						else 
						{
							break;
						}
					}
				default: 
					{
						break;
					}
			}
			/* not a constant in the right range: put it in a register */
			return luaK_exp2anyreg(fs, e);
		}


		public static void luaK_storevar(LuaParser.FuncState fs, LuaParser.expdesc var, LuaParser.expdesc ex)
		{
			switch (var.k) 
			{
				case LuaParser.expkind.VLOCAL:
					{
						freeexp(fs, ex);
						exp2reg(fs, ex, var.u.s.info);
						return;
					}
				case LuaParser.expkind.VUPVAL:
					{
						int e = luaK_exp2anyreg(fs, ex);
						luaK_codeABC(fs, LuaOpCodes.OpCode.OP_SETUPVAL, e, var.u.s.info, 0);
						break;
					}
				case LuaParser.expkind.VGLOBAL:
					{
						int e = luaK_exp2anyreg(fs, ex);
						luaK_codeABx(fs, LuaOpCodes.OpCode.OP_SETGLOBAL, e, var.u.s.info);
						break;
					}
				case LuaParser.expkind.VINDEXED:
					{
						int e = luaK_exp2RK(fs, ex);
						luaK_codeABC(fs, LuaOpCodes.OpCode.OP_SETTABLE, var.u.s.info, var.u.s.aux, e);
						break;
					}
				default: 
					{
						LuaLimits.lua_assert(0);  /* invalid var kind to store */
						break;
					}
			}
			freeexp(fs, ex);
		}


		public static void luaK_self(LuaParser.FuncState fs, LuaParser.expdesc e, LuaParser.expdesc key)
		{
			int func;
			luaK_exp2anyreg(fs, e);
			freeexp(fs, e);
			func = fs.freereg;
			luaK_reserveregs(fs, 2);
			luaK_codeABC(fs, LuaOpCodes.OpCode.OP_SELF, func, e.u.s.info, luaK_exp2RK(fs, key));
			freeexp(fs, key);
			e.u.s.info = func;
			e.k = LuaParser.expkind.VNONRELOC;
		}

		private static void invertjump(LuaParser.FuncState fs, LuaParser.expdesc e)
		{
			InstructionPtr pc = getjumpcontrol(fs, e.u.s.info);
			LuaLimits.lua_assert(LuaOpCodes.testTMode(LuaOpCodes.GET_OPCODE(pc.get(0))) != 0 && LuaOpCodes.GET_OPCODE(pc.get(0)) != LuaOpCodes.OpCode.OP_TESTSET &&
				LuaOpCodes.GET_OPCODE(pc.get(0)) != LuaOpCodes.OpCode.OP_TEST);
			LuaOpCodes.SETARG_A(pc, (LuaOpCodes.GETARG_A(pc.get(0)) == 0) ? 1 : 0);
		}


		private static int jumponcond(LuaParser.FuncState fs, LuaParser.expdesc e, int cond)
		{
			if (e.k == LuaParser.expkind.VRELOCABLE) 
			{
				InstructionPtr ie = getcode(fs, e);
				if (LuaOpCodes.GET_OPCODE(ie) == LuaOpCodes.OpCode.OP_NOT)
				{
					fs.pc--;  /* remove previous OpCode.OP_NOT */
					return condjump(fs, LuaOpCodes.OpCode.OP_TEST, LuaOpCodes.GETARG_B(ie), 0, (cond == 0) ? 1 : 0);
				}
				/* else go through */
			}
			discharge2anyreg(fs, e);
			freeexp(fs, e);
			return condjump(fs, LuaOpCodes.OpCode.OP_TESTSET, LuaOpCodes.NO_REG, e.u.s.info, cond);
		}

		public static void luaK_goiftrue(LuaParser.FuncState fs, LuaParser.expdesc e)
		{
			int pc;  /* pc of last jump */
			luaK_dischargevars(fs, e);
			switch (e.k) 
			{
				case LuaParser.expkind.VK: 
				case LuaParser.expkind.VKNUM: 
				case LuaParser.expkind.VTRUE: 
					{
						pc = NO_JUMP;  /* always true; do nothing */
						break;
					}
				case LuaParser.expkind.VFALSE: 
					{
						pc = luaK_jump(fs);  /* always jump */
						break;
					}
				case LuaParser.expkind.VJMP: 
					{
						invertjump(fs, e);
						pc = e.u.s.info;
						break;
					}
				default: 
					{
						pc = jumponcond(fs, e, 0);
						break;
					}
			}
			int[] f_ref = new int[1];
			f_ref[0] = e.f;
			luaK_concat(fs, /*ref*/ f_ref, pc);  /* insert last jump in `f' list */
			e.f = f_ref[0];
			luaK_patchtohere(fs, e.t);
			e.t = NO_JUMP;
		}

		private static void luaK_goiffalse(LuaParser.FuncState fs, LuaParser.expdesc e)
		{
			int pc;  /* pc of last jump */
			luaK_dischargevars(fs, e);
			switch (e.k) 
			{
				case LuaParser.expkind.VNIL:
				case LuaParser.expkind.VFALSE:
					{
						pc = LuaCode.NO_JUMP;  /* always false; do nothing */
						break;
					}
				case LuaParser.expkind.VTRUE:
					{
						pc = luaK_jump(fs);  /* always jump */
						break;
					}
				case LuaParser.expkind.VJMP:
					{
						pc = e.u.s.info;
						break;
					}
				default: 
					{
						pc = jumponcond(fs, e, 1);
						break;
					}
			}
			int[] t_ref = new int[1];
			t_ref[0] = e.t;
			luaK_concat(fs, /*ref*/ t_ref, pc);  /* insert last jump in `t' list */
			e.t = t_ref[0];
			luaK_patchtohere(fs, e.f);
			e.f = NO_JUMP;
		}

		private static void codenot(LuaParser.FuncState fs, LuaParser.expdesc e)
		{
			luaK_dischargevars(fs, e);
			switch (e.k) 
			{
				case LuaParser.expkind.VNIL: 
				case LuaParser.expkind.VFALSE: 
					{
						e.k = LuaParser.expkind.VTRUE;
						break;
					}
				case LuaParser.expkind.VK: 
				case LuaParser.expkind.VKNUM: 
				case LuaParser.expkind.VTRUE: 
					{
						e.k = LuaParser.expkind.VFALSE;
						break;
					}
				case LuaParser.expkind.VJMP: {
					invertjump(fs, e);
					break;
				}
				case LuaParser.expkind.VRELOCABLE:
				case LuaParser.expkind.VNONRELOC: 
					{
						discharge2anyreg(fs, e);
						freeexp(fs, e);
						e.u.s.info = luaK_codeABC(fs, LuaOpCodes.OpCode.OP_NOT, 0, e.u.s.info, 0);
						e.k = LuaParser.expkind.VRELOCABLE;
						break;
					}
				default: 
					{
						LuaLimits.lua_assert(0);  /* cannot happen */
						break;
					}
			}
			
            //
			// interchange true and false lists
			//

            if (true)
			{
				int temp = e.f; 
				e.f = e.t; 
				e.t = temp; 
			}
			removevalues(fs, e.f);
			removevalues(fs, e.t);
		}

		public static void luaK_indexed(LuaParser.FuncState fs, LuaParser.expdesc t, LuaParser.expdesc k)
		{
			t.u.s.aux = luaK_exp2RK(fs, k);
			t.k = LuaParser.expkind.VINDEXED;
		}

		private static int constfolding(LuaOpCodes.OpCode op, LuaParser.expdesc e1, LuaParser.expdesc e2)
		{
			Double/*lua_Number*/ v1, v2, r;
			if ((isnumeral(e1)==0) || (isnumeral(e2)==0)) 
			{
				return 0;
			}
			v1 = e1.u.nval;
			v2 = e2.u.nval;
			switch (op) 
			{
				case LuaOpCodes.OpCode.OP_ADD: 
					{
						r = LuaConf.luai_numadd(v1, v2); 
						break;
					}
				case LuaOpCodes.OpCode.OP_SUB: 
					{
						r = LuaConf.luai_numsub(v1, v2); 
						break;
					}
				case LuaOpCodes.OpCode.OP_MUL: 
					{
						r = LuaConf.luai_nummul(v1, v2); 
						break;
					}
				case LuaOpCodes.OpCode.OP_DIV:
					{
						if (v2 == 0) 
						{
							return 0;  /* do not attempt to divide by 0 */
						}
						r = LuaConf.luai_numdiv(v1, v2); 
						break;
					}
				case LuaOpCodes.OpCode.OP_MOD:
					{
						if (v2 == 0)
						{
							return 0;  /* do not attempt to divide by 0 */
						}
						r = LuaConf.luai_nummod(v1, v2); 
						break;
					}
				case LuaOpCodes.OpCode.OP_POW: 
					{
						r = LuaConf.luai_numpow(v1, v2); 
						break;
					}
				case LuaOpCodes.OpCode.OP_UNM: 
					{
						r = LuaConf.luai_numunm(v1); 
						break;
					}
				case LuaOpCodes.OpCode.OP_LEN: 
					{
						return 0;  /* no constant folding for 'len' */
					}
				default: 
					{
						LuaLimits.lua_assert(0); 
						r = 0; 
						break;
					}
			}
			if (LuaConf.luai_numisnan(r)) 
			{
				return 0;  /* do not attempt to produce NaN */
			}
			e1.u.nval = r;
			return 1;
		}

		private static void codearith(LuaParser.FuncState fs, LuaOpCodes.OpCode op, LuaParser.expdesc e1, LuaParser.expdesc e2)
		{
			if (constfolding(op, e1, e2) != 0)
			{
				return;
			}
			else 
			{
				int o2 = (op != LuaOpCodes.OpCode.OP_UNM && op != LuaOpCodes.OpCode.OP_LEN) ? luaK_exp2RK(fs, e2) : 0;
				int o1 = luaK_exp2RK(fs, e1);
				if (o1 > o2) 
				{
					freeexp(fs, e1);
					freeexp(fs, e2);
				}
				else 
				{
					freeexp(fs, e2);
					freeexp(fs, e1);
				}
				e1.u.s.info = luaK_codeABC(fs, op, 0, o1, o2);
				e1.k = LuaParser.expkind.VRELOCABLE;
			}
		}

		private static void codecomp(LuaParser.FuncState fs, LuaOpCodes.OpCode op, int cond, LuaParser.expdesc e1, LuaParser.expdesc e2)
		{
			int o1 = luaK_exp2RK(fs, e1);
			int o2 = luaK_exp2RK(fs, e2);
			freeexp(fs, e2);
			freeexp(fs, e1);
			if (cond == 0 && op != LuaOpCodes.OpCode.OP_EQ) 
			{
				int temp;  /* exchange args to replace by `<' or `<=' */
				temp = o1; 
				o1 = o2; 
				o2 = temp;  /* o1 <==> o2 */
				cond = 1;
			}
			e1.u.s.info = condjump(fs, op, cond, o1, o2);
			e1.k = LuaParser.expkind.VJMP;
		}


		public static void luaK_prefix(LuaParser.FuncState fs, UnOpr op, LuaParser.expdesc e)
		{
			LuaParser.expdesc e2 = new LuaParser.expdesc();
			e2.t = e2.f = NO_JUMP; 
			e2.k = LuaParser.expkind.VKNUM; 
			e2.u.nval = 0;
			switch (op) 
			{
				case UnOpr.OPR_MINUS: 
					{
						if (isnumeral(e)==0)
						{
							luaK_exp2anyreg(fs, e);  /* cannot operate on non-numeric constants */
						}
						codearith(fs, LuaOpCodes.OpCode.OP_UNM, e, e2);
						break;
					}
				case UnOpr.OPR_NOT: 
					{
						codenot(fs, e); 
						break;
					}
				case UnOpr.OPR_LEN: 
					{
						luaK_exp2anyreg(fs, e);  /* cannot operate on constants */
						codearith(fs, LuaOpCodes.OpCode.OP_LEN, e, e2);
						break;
					}
				default: 
					{
						LuaLimits.lua_assert(0); 
						break;
					}
			}
		}


		public static void luaK_infix(LuaParser.FuncState fs, BinOpr op, LuaParser.expdesc v)
		{
			switch (op) 
			{
				case BinOpr.OPR_AND: 
					{
						luaK_goiftrue(fs, v);
						break;
					}
				case BinOpr.OPR_OR: 
					{
						luaK_goiffalse(fs, v);
						break;
					}
				case BinOpr.OPR_CONCAT: 
					{
						luaK_exp2nextreg(fs, v);  /* operand must be on the `stack' */
						break;
					}
				case BinOpr.OPR_ADD: 
				case BinOpr.OPR_SUB: 
				case BinOpr.OPR_MUL: 
				case BinOpr.OPR_DIV:
				case BinOpr.OPR_MOD: 
				case BinOpr.OPR_POW: 
					{
						if ((isnumeral(v)==0)) 
						{
							luaK_exp2RK(fs, v);
						}
						break;
					}
				default: 
					{
						luaK_exp2RK(fs, v);
						break;
					}
			}
		}


		public static void luaK_posfix(LuaParser.FuncState fs, BinOpr op, LuaParser.expdesc e1, LuaParser.expdesc e2)
		{
			switch (op) 
			{
				case BinOpr.OPR_AND: 
					{
						LuaLimits.lua_assert(e1.t == NO_JUMP);  /* list must be closed */
						luaK_dischargevars(fs, e2);
						int[] f_ref = new int[1];
						f_ref[0] = e2.f;
						luaK_concat(fs, /*ref*/ f_ref, e1.f);
						e2.f = f_ref[0];
						e1.Copy(e2);
						break;
					}
				case BinOpr.OPR_OR: 
					{
						LuaLimits.lua_assert(e1.f == NO_JUMP);  /* list must be closed */
						luaK_dischargevars(fs, e2);
						int[] t_ref = new int[1];
						t_ref[0] = e2.t;
						luaK_concat(fs, /*ref*/ t_ref, e1.t);
						e2.t = t_ref[0];
						e1.Copy(e2);
						break;
					}
				case BinOpr.OPR_CONCAT: 
					{
						luaK_exp2val(fs, e2);
						if (e2.k == LuaParser.expkind.VRELOCABLE && LuaOpCodes.GET_OPCODE(getcode(fs, e2)) == LuaOpCodes.OpCode.OP_CONCAT)
						{
							LuaLimits.lua_assert(e1.u.s.info == LuaOpCodes.GETARG_B(getcode(fs, e2)) - 1);
							freeexp(fs, e1);
							LuaOpCodes.SETARG_B(getcode(fs, e2), e1.u.s.info);
							e1.k = LuaParser.expkind.VRELOCABLE; e1.u.s.info = e2.u.s.info;
						}
						else 
						{
							luaK_exp2nextreg(fs, e2);  /* operand must be on the 'stack' */
							codearith(fs, LuaOpCodes.OpCode.OP_CONCAT, e1, e2);
						}
						break;
					}
				case BinOpr.OPR_ADD: 
					{
						codearith(fs, LuaOpCodes.OpCode.OP_ADD, e1, e2); 
						break;
					}
				case BinOpr.OPR_SUB: 
					{
						codearith(fs, LuaOpCodes.OpCode.OP_SUB, e1, e2); 
						break;
					}
				case BinOpr.OPR_MUL: 
					{
						codearith(fs, LuaOpCodes.OpCode.OP_MUL, e1, e2); 
						break;
					}
				case BinOpr.OPR_DIV: 
					{
						codearith(fs, LuaOpCodes.OpCode.OP_DIV, e1, e2); 
						break;
					}
				case BinOpr.OPR_MOD: 
					{
						codearith(fs, LuaOpCodes.OpCode.OP_MOD, e1, e2); 
						break;
					}
				case BinOpr.OPR_POW: 
					{
						codearith(fs, LuaOpCodes.OpCode.OP_POW, e1, e2); 
						break;
					}
				case BinOpr.OPR_EQ: 
					{
						codecomp(fs, LuaOpCodes.OpCode.OP_EQ, 1, e1, e2); 
						break;
					}
				case BinOpr.OPR_NE: 
					{
						codecomp(fs, LuaOpCodes.OpCode.OP_EQ, 0, e1, e2); 
						break;
					}
				case BinOpr.OPR_LT: 
					{
						codecomp(fs, LuaOpCodes.OpCode.OP_LT, 1, e1, e2); 
						break;
					}
				case BinOpr.OPR_LE: 
					{
						codecomp(fs, LuaOpCodes.OpCode.OP_LE, 1, e1, e2); 
						break;
					}
				case BinOpr.OPR_GT: 
					{
						codecomp(fs, LuaOpCodes.OpCode.OP_LT, 0, e1, e2); 
						break;
					}
				case BinOpr.OPR_GE: 
					{
						codecomp(fs, LuaOpCodes.OpCode.OP_LE, 0, e1, e2); 
						break;
					}
				default: 
					{
						LuaLimits.lua_assert(0); 
						break;
					}
			}
		}

		public static void luaK_fixline(LuaParser.FuncState fs, int line)
		{
			fs.f.lineinfo[fs.pc - 1] = line;
		}

		private static int luaK_code(LuaParser.FuncState fs, int i, int line)
		{
			LuaObject.Proto f = fs.f;
			dischargejpc(fs);  /* `pc' will change */
			/* put new instruction in code array */
			long[][] code_ref = new long[1][];
			code_ref[0] = f.code;
			int[] sizecode_ref = new int[1];
			sizecode_ref[0] = f.sizecode;
			LuaMem.luaM_growvector_long(fs.L, /*ref*/ code_ref, fs.pc, /*ref*/ sizecode_ref,
				LuaLimits.MAX_INT, CLib.CharPtr.toCharPtr("code size overflow"), new ClassType(ClassType.TYPE_LONG));
			f.sizecode = sizecode_ref[0];
			f.code = code_ref[0];
			f.code[fs.pc] = (long/*uint*/)i;
			/* save corresponding line information */
			int[][] lineinfo_ref = new int[1][];
			lineinfo_ref[0] = f.lineinfo;
			int[] sizelineinfo_ref = new int[1];
			sizelineinfo_ref[0] = f.sizelineinfo;
			LuaMem.luaM_growvector_int(fs.L, /*ref*/ lineinfo_ref, fs.pc, /*ref*/ sizelineinfo_ref,
				LuaLimits.MAX_INT, CLib.CharPtr.toCharPtr("code size overflow"), new ClassType(ClassType.TYPE_INT));
			f.sizelineinfo = sizelineinfo_ref[0];
			f.lineinfo = lineinfo_ref[0];
			f.lineinfo[fs.pc] = line;
			return fs.pc++;
		}

		public static int luaK_codeABC(LuaParser.FuncState fs, LuaOpCodes.OpCode o, int a, int b, int c)
		{
			LuaLimits.lua_assert(LuaOpCodes.getOpMode(o) == LuaOpCodes.OpMode.iABC);
			LuaLimits.lua_assert(LuaOpCodes.getBMode(o) != LuaOpCodes.OpArgMask.OpArgN || b == 0);
			LuaLimits.lua_assert(LuaOpCodes.getCMode(o) != LuaOpCodes.OpArgMask.OpArgN || c == 0);
			return luaK_code(fs, LuaOpCodes.CREATE_ABC(o, a, b, c), fs.ls.lastline);
		}

		public static int luaK_codeABx(LuaParser.FuncState fs, LuaOpCodes.OpCode o, int a, int bc)
		{
			LuaLimits.lua_assert(LuaOpCodes.getOpMode(o) == LuaOpCodes.OpMode.iABx || LuaOpCodes.getOpMode(o) == LuaOpCodes.OpMode.iAsBx);
			LuaLimits.lua_assert(LuaOpCodes.getCMode(o) == LuaOpCodes.OpArgMask.OpArgN);
			return luaK_code(fs, LuaOpCodes.CREATE_ABx(o, a, bc), fs.ls.lastline);
		}

		public static void luaK_setlist(LuaParser.FuncState fs, int base_, int nelems, int tostore)
		{
			int c = (nelems - 1) / LuaOpCodes.LFIELDS_PER_FLUSH + 1;
			int b = (tostore == Lua.LUA_MULTRET) ? 0 : tostore;
			LuaLimits.lua_assert(tostore != 0);
			if (c <= LuaOpCodes.MAXARG_C)
			{
				luaK_codeABC(fs, LuaOpCodes.OpCode.OP_SETLIST, base_, b, c);
			}
			else 
			{
				luaK_codeABC(fs, LuaOpCodes.OpCode.OP_SETLIST, base_, b, 0);
				luaK_code(fs, c, fs.ls.lastline);
			}
			fs.freereg = base_ + 1;  /* free registers with list values */
		}
	}
}
