package kurumi;

//
// ** $Id: ldebug.c,v 2.29.1.6 2008/05/08 16:56:26 roberto Exp $
// ** Debug Interface
// ** See Copyright Notice in lua.h
// 

//using TValue = Lua.TValue;
//using StkId = TValue;
//using Instruction = System.UInt32;

public class LuaDebug {
	public static int pcRel(InstructionPtr pc, Proto p) {
		ClassType.Assert(pc.codes == p.code);
		return pc.pc - 1;
	}

	public static int getline(Proto f, int pc) {
		return (f.lineinfo != null) ? f.lineinfo[pc] : 0;
	}

	public static void resethookcount(lua_State L) {
		L.hookcount = L.basehookcount;
	}

	private static int currentpc(lua_State L, LuaState.CallInfo ci) {
		if (!LuaState.isLua(ci)) {
			return -1; // function is not a Lua function? 
		}
		if (ci == L.ci) {
			ci.savedpc = InstructionPtr.Assign(L.savedpc);
		}
		return pcRel(ci.savedpc, LuaState.ci_func(ci).l.p);
	}

	private static int currentline(lua_State L, LuaState.CallInfo ci) {
		int pc = currentpc(L, ci);
		if (pc < 0) {
			return -1; // only active lua functions have current-line information 
		}
		else {
			return getline(LuaState.ci_func(ci).l.p, pc);
		}
	}

//        
//		 ** this function can be called asynchronous (e.g. during a signal)
//		 
	public static int lua_sethook(lua_State L, lua_Hook func, int mask, int count) {
		if (func == null || mask == 0) {
			// turn off hooks? 
			mask = 0;
			func = null;
		}
		L.hook = func;
		L.basehookcount = count;
		resethookcount(L);
		L.hookmask = LuaLimits.cast_byte(mask);
		return 1;
	}

	public static lua_Hook lua_gethook(lua_State L) {
		return L.hook;
	}

	public static int lua_gethookmask(lua_State L) {
		return L.hookmask;
	}

	public static int lua_gethookcount(lua_State L) {
		return L.basehookcount;
	}

	public static int lua_getstack(lua_State L, int level, lua_Debug ar) {
		int status;
		LuaState.CallInfo[] ci = new LuaState.CallInfo[1];
		ci[0] = new LuaState.CallInfo();
		LuaLimits.lua_lock(L);
		for (ci[0] = L.ci; level > 0 && LuaState.CallInfo.greaterThan(ci[0], L.base_ci[0]); LuaState.CallInfo.dec(ci)) { //ref
			level--;
			if (LuaState.f_isLua(ci[0])) { // Lua function? 
				level -= ci[0].tailcalls; // skip lost tail calls 
			}
		}
		if (level == 0 && LuaState.CallInfo.greaterThan(ci[0], L.base_ci[0])) {
			// level found? 
			status = 1;
			ar.i_ci = LuaState.CallInfo.minus(ci[0], L.base_ci[0]);
		}
		else if (level < 0) {
			// level is of a lost tail call? 
			status = 1;
			ar.i_ci = 0;
		}
		else {
			status = 0; // no such level 
		}
		LuaLimits.lua_unlock(L);
		return status;
	}


	private static Proto getluaproto(LuaState.CallInfo ci) {
		return (LuaState.isLua(ci) ? LuaState.ci_func(ci).l.p : null);
	}

	private static LuaConf.CharPtr findlocal(lua_State L, LuaState.CallInfo ci, int n) {
		LuaConf.CharPtr name;
		Proto fp = getluaproto(ci);
		if ((fp != null) && LuaConf.CharPtr.isNotEqual((name = LuaFunc.luaF_getlocalname(fp, n, currentpc(L, ci))), null)) {
			return name; // is a local variable in a Lua function 
		}
		else {
			TValue limit = (ci == L.ci) ? L.top : (LuaState.CallInfo.plus(ci, 1)).func; //StkId
			if (TValue.minus(limit, ci.base_) >= n && n > 0) { // is 'n' inside 'ci' stack? 
				return LuaConf.CharPtr.toCharPtr("(*temporary)");
			}
			else {
				return null;
			}
		}
	}

	public static LuaConf.CharPtr lua_getlocal(lua_State L, lua_Debug ar, int n) {
		LuaState.CallInfo ci = L.base_ci[ar.i_ci];
		LuaConf.CharPtr name = findlocal(L, ci, n);
		LuaLimits.lua_lock(L);
		if (LuaConf.CharPtr.isNotEqual(name, null)) {
			LuaAPI.luaA_pushobject(L, ci.base_.get(n - 1));
		}
		LuaLimits.lua_unlock(L);
		return name;
	}

	public static LuaConf.CharPtr lua_setlocal(lua_State L, lua_Debug ar, int n) {
		LuaState.CallInfo ci = L.base_ci[ar.i_ci];
		LuaConf.CharPtr name = findlocal(L, ci, n);
		LuaLimits.lua_lock(L);
		if (LuaConf.CharPtr.isNotEqual(name, null)) {
			LuaObject.setobjs2s(L, ci.base_.get(n - 1), TValue.minus(L.top, 1));
		}
		TValue[] top = new TValue[1];
		top[0] = L.top;
		//StkId
TValue.dec(top); // pop value  - ref
		L.top = top[0];
		LuaLimits.lua_unlock(L);
		return name;
	}

	private static void funcinfo(lua_Debug ar, Closure cl) {
		if (cl.c.getIsC() != 0) {
			ar.source = LuaConf.CharPtr.toCharPtr("=[C]");
			ar.linedefined = -1;
			ar.lastlinedefined = -1;
			ar.what = LuaConf.CharPtr.toCharPtr("C");
		}
		else {
			ar.source = LuaObject.getstr(cl.l.p.source);
			ar.linedefined = cl.l.p.linedefined;
			ar.lastlinedefined = cl.l.p.lastlinedefined;
			ar.what = (ar.linedefined == 0) ? LuaConf.CharPtr.toCharPtr("main") : LuaConf.CharPtr.toCharPtr("Lua");
		}
		LuaObject.luaO_chunkid(ar.short_src, ar.source, LuaConf.LUA_IDSIZE);
	}

	private static void info_tailcall(lua_Debug ar) {
		ar.name = ar.namewhat = LuaConf.CharPtr.toCharPtr("");
		ar.what = LuaConf.CharPtr.toCharPtr("tail");
		ar.lastlinedefined = ar.linedefined = ar.currentline = -1;
		ar.source = LuaConf.CharPtr.toCharPtr("=(tail call)");
		LuaObject.luaO_chunkid(ar.short_src, ar.source, LuaConf.LUA_IDSIZE);
		ar.nups = 0;
	}

	private static void collectvalidlines(lua_State L, Closure f) {
		if (f == null || (f.c.getIsC() != 0)) {
			LuaObject.setnilvalue(L.top);
		}
		else {
			Table t = LuaTable.luaH_new(L, 0, 0);
			int[] lineinfo = f.l.p.lineinfo;
			int i;
			for (i=0; i<f.l.p.sizelineinfo; i++) {
				LuaObject.setbvalue(LuaTable.luaH_setnum(L, t, lineinfo[i]), 1);
			}
			LuaObject.sethvalue(L, L.top, t);
		}
		LuaDo.incr_top(L);
	}

	private static int auxgetinfo(lua_State L, LuaConf.CharPtr what, lua_Debug ar, Closure f, LuaState.CallInfo ci) {
		int status = 1;
		if (f == null) {
			info_tailcall(ar);
			return status;
		}
		for (; what.get(0) != 0; what = what.next()) {
			switch (what.get(0)) {
				case 'S': {
						funcinfo(ar, f);
						break;
					}
				case 'l': {
						ar.currentline = (ci != null) ? currentline(L, ci) : -1;
						break;
					}
				case 'u': {
						ar.nups = f.c.getNupvalues();
						break;
					}
				case 'n': {
						LuaConf.CharPtr[] name_ref = new LuaConf.CharPtr[1];
						name_ref[0] = ar.name;
						ar.namewhat = (ci != null) ? getfuncname(L, ci, name_ref) : null; //ref
						ar.name = name_ref[0];
						if (LuaConf.CharPtr.isEqual(ar.namewhat, null)) {
							ar.namewhat = LuaConf.CharPtr.toCharPtr(""); // not found 
							ar.name = null;
						}
						break;
					}
				case 'L':
				case 'f': { // handled by lua_getinfo 
						break;
					}
				default: {
						status = 0;
						break; // invalid option 
					}
			}
		}
		return status;
	}

	public static int lua_getinfo(lua_State L, LuaConf.CharPtr what, lua_Debug ar) {
		int status;
		Closure f = null;
		LuaState.CallInfo ci = null;
		LuaLimits.lua_lock(L);
		if (LuaConf.CharPtr.isEqualChar(what, '>')) {
			TValue func = TValue.minus(L.top, 1); //StkId
			LuaConf.luai_apicheck(L, LuaObject.ttisfunction(func));
			what = what.next(); // skip the '>' 
			f = LuaObject.clvalue(func);
			TValue[] top = new TValue[1];
			top[0] = L.top;
			//StkId
TValue.dec(top); // pop function  - ref
			L.top = top[0];
		}
		else if (ar.i_ci != 0) {
			// no tail call? 
			ci = L.base_ci[ar.i_ci];
			LuaLimits.lua_assert(LuaObject.ttisfunction(ci.func));
			f = LuaObject.clvalue(ci.func);
		}
		status = auxgetinfo(L, what, ar, f, ci);
		if (LuaConf.CharPtr.isNotEqual(LuaConf.strchr(what, 'f'), null)) {
			if (f == null) {
				LuaObject.setnilvalue(L.top);
			}
			else {
				LuaObject.setclvalue(L, L.top, f);
			}
			LuaDo.incr_top(L);
		}
		if (LuaConf.CharPtr.isNotEqual(LuaConf.strchr(what, 'L'), null)) {
			collectvalidlines(L, f);
		}
		LuaLimits.lua_unlock(L);
		return status;
	}

//        
//		 ** {======================================================
//		 ** Symbolic Execution and code checker
//		 ** =======================================================
//		 

	private static int checkjump(Proto pt, int pc) {
		if (!(0 <= pc && pc < pt.sizecode)) {
			return 0;
		}
		return 1;
	}

	private static int checkreg(Proto pt, int reg) {
		if (!((reg) < (pt).maxstacksize)) {
			return 0;
		}
		return 1;
	}

	private static int precheck(Proto pt) {
		if (!(pt.maxstacksize <= LuaLimits.MAXSTACK)) {
			return 0;
		}
		if (!(pt.numparams + (pt.is_vararg & LuaObject.VARARG_HASARG) <= pt.maxstacksize)) {
			return 0;
		}
		if (!(((pt.is_vararg & LuaObject.VARARG_NEEDSARG) == 0) || ((pt.is_vararg & LuaObject.VARARG_HASARG) != 0))) {
			return 0;
		}
		if (!(pt.sizeupvalues <= pt.nups)) {
			return 0;
		}
		if (!(pt.sizelineinfo == pt.sizecode || pt.sizelineinfo == 0)) {
			return 0;
		}
		if (!(pt.sizecode > 0 && LuaOpCodes.GET_OPCODE(pt.code[pt.sizecode - 1]) == OpCode.OP_RETURN)) {
			return 0;
		}
		return 1;
	}

	public static int checkopenop(Proto pt, int pc) {
		return luaG_checkopenop(pt.code[pc + 1]);
	}

	public static int luaG_checkopenop(long i) { //Instruction - UInt32
		switch (LuaOpCodes.GET_OPCODE(i)) {
			case OP_CALL:
			case OP_TAILCALL:
			case OP_RETURN:
			case OP_SETLIST: {
					if (!(LuaOpCodes.GETARG_B(i) == 0)) {
						return 0;
					}
					return 1;
				}
			default: {
					return 0; // invalid instruction after an open call 
				}
		}
	}


	private static int checkArgMode(Proto pt, int r, OpArgMask mode) {
		switch (mode) {
			case OpArgN: {
					if (r!=0) {
						return 0;
					}
					break;
				}
			case OpArgU: {
					break;
				}
			case OpArgR: {
					checkreg(pt, r);
					break;
				}
			case OpArgK: {
					if (!((LuaOpCodes.ISK(r) != 0) ? LuaOpCodes.INDEXK(r) < pt.sizek : r < pt.maxstacksize)) {
						return 0;
					}
					break;
				}
		}
		return 1;
	}

	private static long symbexec(Proto pt, int lastpc, int reg) { //Instruction - UInt32
		int pc;
		int last; // stores position of last instruction that changed `reg' 
		int dest;
		last = pt.sizecode - 1; // points to final return (a `neutral' instruction) 
		if (precheck(pt)==0) {
			return 0;
		}
		for (pc = 0; pc < lastpc; pc++) {
			long i = pt.code[pc]; //Instruction - UInt32
			OpCode op = LuaOpCodes.GET_OPCODE(i);
			int a = LuaOpCodes.GETARG_A(i);
			int b = 0;
			int c = 0;
			if (!(op.getValue() < LuaOpCodes.NUM_OPCODES)) {
				return 0;
			}
			checkreg(pt, a);
			switch (LuaOpCodes.getOpMode(op)) {
				case iABC: {
						b = LuaOpCodes.GETARG_B(i);
						c = LuaOpCodes.GETARG_C(i);
						if (checkArgMode(pt, b, LuaOpCodes.getBMode(op)) == 0) {
							return 0;
						}
						if (checkArgMode(pt, c, LuaOpCodes.getCMode(op)) == 0) {
							return 0;
						}
						break;
					}
				case iABx: {
						b = LuaOpCodes.GETARG_Bx(i);
						if (LuaOpCodes.getBMode(op) == OpArgMask.OpArgK) {
							if (!(b < pt.sizek)) {
								return 0;
							}
						}
						break;
					}
				case iAsBx: {
						b = LuaOpCodes.GETARG_sBx(i);
						if (LuaOpCodes.getBMode(op) == OpArgMask.OpArgR) {
							dest = pc + 1 + b;
							if (!((0 <= dest && dest < pt.sizecode))) {
								return 0;
							}
							if (dest > 0) {
								int j;
//                                     check that it does not jump to a setlist count; this
//					   is tricky, because the count from a previous setlist may
//					   have the same value of an invalid setlist; so, we must
//					   go all the way back to the first of them (if any) 
								for (j = 0; j < dest; j++) {
									long d = pt.code[dest - 1 - j]; //Instruction - UInt32
									if (!(LuaOpCodes.GET_OPCODE(d) == OpCode.OP_SETLIST && LuaOpCodes.GETARG_C(d) == 0)) {
										break;
									}
								}
//                                     if 'j' is even, previous value is not a setlist (even if
//					   it looks like one) 
								if ((j & 1) != 0) {
									return 0;
								}
							}
						}
						break;
					}
			}
			if (LuaOpCodes.testAMode(op) != 0) {
				if (a == reg) {
					last = pc; // change register `a' 
				}
			}
			if (LuaOpCodes.testTMode(op) != 0) {
				if (!(pc+2 < pt.sizecode)) {
					return 0; // check skip 
				}
				if (!(LuaOpCodes.GET_OPCODE(pt.code[pc + 1]) == OpCode.OP_JMP)) {
					return 0;
				}
			}
			switch (op) {
				case OP_LOADBOOL: {
						if (c == 1) {
							// does it jump? 
							if (!(pc+2 < pt.sizecode)) {
								return 0; // check its jump 
							}
							if (!(LuaOpCodes.GET_OPCODE(pt.code[pc + 1]) != OpCode.OP_SETLIST || LuaOpCodes.GETARG_C(pt.code[pc + 1]) != 0)) {
								return 0;
							}
						}
						break;
					}
				case OP_LOADNIL: {
						if (a <= reg && reg <= b) {
							last = pc; // set registers from `a' to `b' 
						}
						break;
					}
				case OP_GETUPVAL:
				case OP_SETUPVAL: {
						if (!(b < pt.nups)) {
							return 0;
						}
						break;
					}
				case OP_GETGLOBAL:
				case OP_SETGLOBAL: {
						if (!(LuaObject.ttisstring(pt.k[b]))) {
							return 0;
						}
						break;
					}
				case OP_SELF: {
						checkreg(pt, a+1);
						if (reg == a+1) {
							last = pc;
						}
						break;
					}
				case OP_CONCAT: {
						if (!(b < c)) {
							return 0; // at least two operands 
						}
						break;
					}
				case OP_TFORLOOP: {
						if (!(c >= 1)) {
							return 0; // at least one result (control variable) 
						}
						checkreg(pt, a+2+c); // space for results 
						if (reg >= a+2) {
							last = pc; // affect all regs above its base 
						}
						break;
					}
				case OP_FORLOOP:
				case OP_FORPREP: {
						checkreg(pt, a+3);
						// go through ...no, on second thoughts don't, because this is C# 
						dest = pc + 1 + b;
						// not full check and jump is forward and do not skip `lastpc'? 
						if (reg != LuaOpCodes.NO_REG && pc < dest && dest <= lastpc) {
							pc += b; // do the jump 
						}
						break;
					}

				case OP_JMP: {
						dest = pc+1+b;
						// not full check and jump is forward and do not skip `lastpc'? 
						if (reg != LuaOpCodes.NO_REG && pc < dest && dest <= lastpc) {
							pc += b; // do the jump 
						}
						break;
					}
				case OP_CALL:
				case OP_TAILCALL: {
						if (b != 0) {
							checkreg(pt, a+b-1);
						}
						c--; // c = num. returns 
						if (c == Lua.LUA_MULTRET) {
							if (checkopenop(pt, pc)==0) {
								return 0;
							}
						}
						else if (c != 0) {
							checkreg(pt, a+c-1);
						}
						if (reg >= a) {
							last = pc; // affect all registers above base 
						}
						break;
					}
				case OP_RETURN: {
						b--; // b = num. returns 
						if (b > 0) {
							checkreg(pt, a+b-1);
						}
						break;
					}
				case OP_SETLIST: {
						if (b > 0) {
							checkreg(pt, a + b);
						}
						if (c == 0) {
							pc++;
							if (!(pc < pt.sizecode - 1)) {
								return 0;
							}
						}
						break;
					}
				case OP_CLOSURE: {
						int nup, j;
						if (!(b < pt.sizep)) {
							return 0;
						}
						nup = pt.p[b].nups;
						if (!(pc + nup < pt.sizecode)) {
							return 0;
						}
						for (j = 1; j <= nup; j++) {
							OpCode op1 = LuaOpCodes.GET_OPCODE(pt.code[pc + j]);
							if (!(op1 == OpCode.OP_GETUPVAL || op1 == OpCode.OP_MOVE)) {
								return 0;
							}
						}
						if (reg != LuaOpCodes.NO_REG) { // tracing? 
							pc += nup; // do not 'execute' these pseudo-instructions 
						}
						break;
					}
				case OP_VARARG: {
						if (!((pt.is_vararg & LuaObject.VARARG_ISVARARG) != 0 && (pt.is_vararg & LuaObject.VARARG_NEEDSARG) == 0)) {
							return 0;
						}
						b--;
						if (b == Lua.LUA_MULTRET) {
							if (checkopenop(pt, pc) == 0) {
								return 0;
							}
						}
						checkreg(pt, a + b - 1);
						break;
					}
				default: {
						break;
					}
			}
		}
		return pt.code[last];
	}

	///#undef check
	///#undef checkjump
	///#undef checkreg

	// }====================================================== 

	public static int luaG_checkcode(Proto pt) {
		return (symbexec(pt, pt.sizecode, LuaOpCodes.NO_REG) != 0) ? 1 : 0;
	}

	private static LuaConf.CharPtr kname(Proto p, int c) {
		if (LuaOpCodes.ISK(c) != 0 && LuaObject.ttisstring(p.k[LuaOpCodes.INDEXK(c)])) {
			return LuaObject.svalue(p.k[LuaOpCodes.INDEXK(c)]);
		}
		else {
			return LuaConf.CharPtr.toCharPtr("?");
		}
	}


	private static LuaConf.CharPtr getobjname(lua_State L, LuaState.CallInfo ci, int stackpos, LuaConf.CharPtr[] name) { //ref
		if (LuaState.isLua(ci)) {
			// a Lua function? 
			Proto p = LuaState.ci_func(ci).l.p;
			int pc = currentpc(L, ci);
			long i; //Instruction - UInt32
			name[0] = LuaFunc.luaF_getlocalname(p, stackpos + 1, pc);
			if (LuaConf.CharPtr.isNotEqual(name[0], null)) { // is a local? 
				return LuaConf.CharPtr.toCharPtr("local");
			}
			i = symbexec(p, pc, stackpos); // try symbolic execution 
			LuaLimits.lua_assert(pc != -1);
			switch (LuaOpCodes.GET_OPCODE(i)) {
				case OP_GETGLOBAL: {
						int g = LuaOpCodes.GETARG_Bx(i); // global index 
						LuaLimits.lua_assert(LuaObject.ttisstring(p.k[g]));
						name[0] = LuaObject.svalue(p.k[g]);
						return LuaConf.CharPtr.toCharPtr("global");
					}
				case OP_MOVE: {
						int a = LuaOpCodes.GETARG_A(i);
						int b = LuaOpCodes.GETARG_B(i); // move from `b' to `a' 
						if (b < a) {
							return getobjname(L, ci, b, name); // get name for `b'  - ref
						}
						break;
					}
				case OP_GETTABLE: {
						int k = LuaOpCodes.GETARG_C(i); // key index 
						name[0] = kname(p, k);
						return LuaConf.CharPtr.toCharPtr("field");
					}
				case OP_GETUPVAL: {
						int u = LuaOpCodes.GETARG_B(i); // upvalue index 
						name[0] = (p.upvalues != null) ? LuaObject.getstr(p.upvalues[u]) : LuaConf.CharPtr.toCharPtr("?");
						return LuaConf.CharPtr.toCharPtr("upvalue");
					}
				case OP_SELF: {
						int k = LuaOpCodes.GETARG_C(i); // key index 
						name[0] = kname(p, k);
						return LuaConf.CharPtr.toCharPtr("method");
					}
				default: {
						break;
					}
			}
		}
		return null; // no useful name found 
	}

	private static LuaConf.CharPtr getfuncname(lua_State L, LuaState.CallInfo ci, LuaConf.CharPtr[] name) { //ref
		long i; //Instruction - UInt32
		if ((LuaState.isLua(ci) && ci.tailcalls > 0) || !LuaState.isLua(LuaState.CallInfo.minus(ci, 1))) {
			return null; // calling function is not Lua (or is unknown) 
		}
		LuaState.CallInfo[] ci_ref = new LuaState.CallInfo[1];
		ci_ref[0] = ci;
		LuaState.CallInfo.dec(ci_ref); // calling function  - ref
		ci = ci_ref[0];
		i = LuaState.ci_func(ci).l.p.code[currentpc(L, ci)];
		if (LuaOpCodes.GET_OPCODE(i) == OpCode.OP_CALL || LuaOpCodes.GET_OPCODE(i) == OpCode.OP_TAILCALL || LuaOpCodes.GET_OPCODE(i) == OpCode.OP_TFORLOOP) {
			return getobjname(L, ci, LuaOpCodes.GETARG_A(i), name); //ref
		}
		else {
			return null; // no useful name can be found 
		}
	}

	// only ANSI way to check whether a pointer points to an array 
	private static int isinstack(LuaState.CallInfo ci, TValue o) {
		TValue[] p = new TValue[1]; //StkId
		p[0] = new TValue();
		for (p[0] = ci.base_; TValue.lessThan(p[0], ci.top); TValue.inc(p)) { //ref - StkId
			if (o == p[0]) {
				return 1;
			}
		}
		return 0;
	}

	public static void luaG_typeerror(lua_State L, TValue o, LuaConf.CharPtr op) {
		LuaConf.CharPtr name = null;
		LuaConf.CharPtr t = LuaTM.luaT_typenames[LuaObject.ttype(o)];
		LuaConf.CharPtr[] name_ref = new LuaConf.CharPtr[1];
		name_ref[0] = name;
		LuaConf.CharPtr kind = (isinstack(L.ci, o)) != 0 ? getobjname(L, L.ci, LuaLimits.cast_int(TValue.minus(o, L.base_)), name_ref) : null; //ref
		name = name_ref[0];
		if (LuaConf.CharPtr.isNotEqual(kind, null)) {
			luaG_runerror(L, LuaConf.CharPtr.toCharPtr("attempt to %s %s " + LuaConf.getLUA_QS() + " (a %s value)"), op, kind, name, t);
		}
		else {
			luaG_runerror(L, LuaConf.CharPtr.toCharPtr("attempt to %s a %s value"), op, t);
		}
	}

	public static void luaG_concaterror(lua_State L, TValue p1, TValue p2) { //StkId - StkId
		if (LuaObject.ttisstring(p1) || LuaObject.ttisnumber(p1)) {
			p1 = p2;
		}
		LuaLimits.lua_assert(!LuaObject.ttisstring(p1) && !LuaObject.ttisnumber(p1));
		luaG_typeerror(L, p1, LuaConf.CharPtr.toCharPtr("concatenate"));
	}

	public static void luaG_aritherror(lua_State L, TValue p1, TValue p2) {
		TValue temp = new TValue();
		if (LuaVM.luaV_tonumber(p1, temp) == null) {
			p2 = p1; // first operand is wrong 
		}
		luaG_typeerror(L, p2, LuaConf.CharPtr.toCharPtr("perform arithmetic on"));
	}

	public static int luaG_ordererror(lua_State L, TValue p1, TValue p2) {
		LuaConf.CharPtr t1 = LuaTM.luaT_typenames[LuaObject.ttype(p1)];
		LuaConf.CharPtr t2 = LuaTM.luaT_typenames[LuaObject.ttype(p2)];
		if (t1.get(2) == t2.get(2)) {
			luaG_runerror(L, LuaConf.CharPtr.toCharPtr("attempt to compare two %s values"), t1);
		}
		else {
			luaG_runerror(L, LuaConf.CharPtr.toCharPtr("attempt to compare %s with %s"), t1, t2);
		}
		return 0;
	}

	private static void addinfo(lua_State L, LuaConf.CharPtr msg) {
		LuaState.CallInfo ci = L.ci;
		if (LuaState.isLua(ci)) {
			// is Lua code? 
			LuaConf.CharPtr buff = new LuaConf.CharPtr(new char[LuaConf.LUA_IDSIZE]); // add file:line information 
			int line = currentline(L, ci);
			LuaObject.luaO_chunkid(buff, LuaObject.getstr(getluaproto(ci).source), LuaConf.LUA_IDSIZE);
			LuaObject.luaO_pushfstring(L, LuaConf.CharPtr.toCharPtr("%s:%d: %s"), buff, line, msg);
		}
	}

	public static void luaG_errormsg(lua_State L) {
		if (L.errfunc != 0) {
			// is there an error handling function? 
			TValue errfunc = LuaDo.restorestack(L, L.errfunc); //StkId
			if (!LuaObject.ttisfunction(errfunc)) {
				LuaDo.luaD_throw(L, Lua.LUA_ERRERR);
			}
			LuaObject.setobjs2s(L, L.top, TValue.minus(L.top, 1)); // move argument 
			LuaObject.setobjs2s(L, TValue.minus(L.top, 1), errfunc); // push function 
			LuaDo.incr_top(L);
			LuaDo.luaD_call(L, TValue.minus(L.top, 2), 1); // call it 
		}
		LuaDo.luaD_throw(L, Lua.LUA_ERRRUN);
	}

	public static void luaG_runerror(lua_State L, LuaConf.CharPtr fmt, Object... argp) {
		addinfo(L, LuaObject.luaO_pushvfstring(L, fmt, argp));
		luaG_errormsg(L);
	}
}