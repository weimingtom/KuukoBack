package kurumi;

//
// ** $Id: print.c,v 1.55a 2006/05/31 13:30:05 lhf Exp $
// ** print bytecodes
// ** See Copyright Notice in lua.h
// 
//using TValue = Lua.TValue;
//using Instruction = System.UInt32;

public class LuaPrint {
	public static void luaU_print(LuaObject.Proto f, int full) {
		PrintFunction(f, full);
	}

	///#define Sizeof(x)	((int)sizeof(x))
	///#define VOID(p)		((const void*)(p))

	public static void PrintString(LuaObject.TString ts) {
		CLib.CharPtr s = LuaObject.getstr(ts);
		int i, n = ts.getTsv().len; //uint
		CLib.putchar('"');
		for (i = 0; i < n; i++) {
			int c = s.get(i);
			switch (c) {
				case '"': {
						CLib.printf(CLib.CharPtr.toCharPtr("\\\""));
						break;
					}
				case '\\': {
						CLib.printf(CLib.CharPtr.toCharPtr("\\\\"));
						break;
					}
				case '\u0007': { //'\a': //FIXME:
						CLib.printf(CLib.CharPtr.toCharPtr("\\a"));
						break;
					}
				case '\b': {
						CLib.printf(CLib.CharPtr.toCharPtr("\\b"));
						break;
					}
				case '\f': {
						CLib.printf(CLib.CharPtr.toCharPtr("\\f"));
						break;
					}
				case '\n': {
						CLib.printf(CLib.CharPtr.toCharPtr("\\n"));
						break;
					}
				case '\r': {
						CLib.printf(CLib.CharPtr.toCharPtr("\\r"));
						break;
					}
				case '\t': {
						CLib.printf(CLib.CharPtr.toCharPtr("\\t"));
						break;
					}
				case '\u000B': { //case '\v': //FIXME:
						CLib.printf(CLib.CharPtr.toCharPtr("\\v"));
						break;
					}
				default: {
						if (CLib.isprint((byte)c)) {
							CLib.putchar(c);
						}
						else {
							CLib.printf(CLib.CharPtr.toCharPtr("\\%03u"), (byte)c);
						}
						break;
					}
			}
		}
		CLib.putchar('"');
	}

	private static void PrintConstant(LuaObject.Proto f, int i) {
		//const
		LuaObject.TValue o = f.k[i];
		switch (LuaObject.ttype(o)) {
		case Lua.LUA_TNIL: {
				CLib.printf(CLib.CharPtr.toCharPtr("nil"));
				break;
			}
		case Lua.LUA_TBOOLEAN: {
				CLib.printf(LuaObject.bvalue(o) != 0 ? CLib.CharPtr.toCharPtr("true") : CLib.CharPtr.toCharPtr("false"));
				break;
			}
		case Lua.LUA_TNUMBER: {
				CLib.printf(CLib.CharPtr.toCharPtr(LuaConf.LUA_NUMBER_FMT), LuaObject.nvalue(o));
				break;
			}
		case Lua.LUA_TSTRING: {
				PrintString(LuaObject.rawtsvalue(o));
				break;
			}
		default: {
				// cannot happen 
				CLib.printf(CLib.CharPtr.toCharPtr("? type=%d"), LuaObject.ttype(o));
				break;
			}
		}
	}

	private static void PrintCode(LuaObject.Proto f) {
		long[] code = f.code; //Instruction[] - UInt32[]
		int pc, n = f.sizecode;
		for (pc = 0; pc < n; pc++) {
			long i = f.code[pc]; //Instruction - UInt32
			LuaOpCodes.OpCode o = LuaOpCodes.GET_OPCODE(i);
			int a = LuaOpCodes.GETARG_A(i);
			int b = LuaOpCodes.GETARG_B(i);
			int c = LuaOpCodes.GETARG_C(i);
			int bx = LuaOpCodes.GETARG_Bx(i);
			int sbx = LuaOpCodes.GETARG_sBx(i);
			int line = LuaDebug.getline(f, pc);
			CLib.printf(CLib.CharPtr.toCharPtr("\t%d\t"), pc + 1);
			if (line > 0) {
				CLib.printf(CLib.CharPtr.toCharPtr("[%d]\t"), line);
			}
			else {
				CLib.printf(CLib.CharPtr.toCharPtr("[-]\t"));
			}
			CLib.printf(CLib.CharPtr.toCharPtr("%-9s\t"), LuaOpCodes.luaP_opnames[o.getValue()]);
			switch (LuaOpCodes.getOpMode(o)) {
				case iABC: {
						CLib.printf(CLib.CharPtr.toCharPtr("%d"), a);
						if (LuaOpCodes.getBMode(o) != LuaOpCodes.OpArgMask.OpArgN) {
							CLib.printf(CLib.CharPtr.toCharPtr(" %d"), (LuaOpCodes.ISK(b) != 0) ? (-1 - LuaOpCodes.INDEXK(b)) : b);
						}
						if (LuaOpCodes.getCMode(o) != LuaOpCodes.OpArgMask.OpArgN) {
							CLib.printf(CLib.CharPtr.toCharPtr(" %d"), (LuaOpCodes.ISK(c) != 0) ? (-1 - LuaOpCodes.INDEXK(c)) : c);
						}
						break;
					}
				case iABx: {
						if (LuaOpCodes.getBMode(o) == LuaOpCodes.OpArgMask.OpArgK) {
							CLib.printf(CLib.CharPtr.toCharPtr("%d %d"), a, -1 - bx);
						}
						else {
							CLib.printf(CLib.CharPtr.toCharPtr("%d %d"), a, bx);
						}
						break;
					}
				case iAsBx:
					if (o == LuaOpCodes.OpCode.OP_JMP) {
						CLib.printf(CLib.CharPtr.toCharPtr("%d"), sbx);
					}
					else {
						CLib.printf(CLib.CharPtr.toCharPtr("%d %d"), a, sbx);
					}
					break;
			}
			switch (o) {
				case OP_LOADK: {
						CLib.printf(CLib.CharPtr.toCharPtr("\t; "));
						PrintConstant(f, bx);
						break;
					}
				case OP_GETUPVAL:
				case OP_SETUPVAL: {
						CLib.printf(CLib.CharPtr.toCharPtr("\t; %s"), (f.sizeupvalues > 0) ? LuaObject.getstr(f.upvalues[b]) : CLib.CharPtr.toCharPtr("-"));
						break;
					}
				case OP_GETGLOBAL:
				case OP_SETGLOBAL: {
						CLib.printf(CLib.CharPtr.toCharPtr("\t; %s"), LuaObject.svalue(f.k[bx]));
						break;
					}
				case OP_GETTABLE:
				case OP_SELF: {
						if (LuaOpCodes.ISK(c) != 0) {
							CLib.printf(CLib.CharPtr.toCharPtr("\t; "));
							PrintConstant(f, LuaOpCodes.INDEXK(c));
						}
						break;
					}
				case OP_SETTABLE:
				case OP_ADD:
				case OP_SUB:
				case OP_MUL:
				case OP_DIV:
				case OP_POW:
				case OP_EQ:
				case OP_LT:
				case OP_LE: {
						if (LuaOpCodes.ISK(b) != 0 || LuaOpCodes.ISK(c) != 0) {
							CLib.printf(CLib.CharPtr.toCharPtr("\t; "));
							if (LuaOpCodes.ISK(b) != 0) {
								PrintConstant(f, LuaOpCodes.INDEXK(b));
							}
							else {
								CLib.printf(CLib.CharPtr.toCharPtr("-"));
							}
							CLib.printf(CLib.CharPtr.toCharPtr(" "));
							if (LuaOpCodes.ISK(c) != 0) {
								PrintConstant(f, LuaOpCodes.INDEXK(c));
							}
							else {
								CLib.printf(CLib.CharPtr.toCharPtr("-"));
							}
						}
						break;
					}
				case OP_JMP:
				case OP_FORLOOP:
				case OP_FORPREP: {
						CLib.printf(CLib.CharPtr.toCharPtr("\t; to %d"), sbx + pc + 2);
						break;
					}
				case OP_CLOSURE: {
						CLib.printf(CLib.CharPtr.toCharPtr("\t; %p"), CLib.VOID(f.p[bx]));
						break;
					}
				case OP_SETLIST: {
						if (c == 0) {
							CLib.printf(CLib.CharPtr.toCharPtr("\t; %d"), (int)code[++pc]);
						}
						else {
							CLib.printf(CLib.CharPtr.toCharPtr("\t; %d"), c);
						}
						break;
					}
				default: {
						break;
					}
			}
			CLib.printf(CLib.CharPtr.toCharPtr("\n"));
		}
	}

	public static String SS(int x) {
		return (x == 1) ? "" : "s";
	}

	///#define S(x)	x,SS(x)

	private static void PrintHeader(LuaObject.Proto f) {
		CLib.CharPtr s = LuaObject.getstr(f.source);
		if (s.get(0) == '@' || s.get(0) == '=') {
			s = s.next();
		}
		else if (s.get(0) == Lua.LUA_SIGNATURE.charAt(0)) {
			s = CLib.CharPtr.toCharPtr("(bstring)");
		}
		else {
			s = CLib.CharPtr.toCharPtr("(string)");
		}
		CLib.printf(CLib.CharPtr.toCharPtr("\n%s <%s:%d,%d> (%d Instruction%s, %d bytes at %p)\n"), (f.linedefined == 0) ? "main" : "function", s, f.linedefined, f.lastlinedefined, f.sizecode, SS(f.sizecode), f.sizecode * CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_LONG)), CLib.VOID(f));
		//typeof(long/*UInt32*//*Instruction*/)
		CLib.printf(CLib.CharPtr.toCharPtr("%d%s param%s, %d slot%s, %d upvalue%s, "), f.numparams, (f.is_vararg != 0) ? "+" : "", SS(f.numparams), f.maxstacksize, SS(f.maxstacksize), f.nups, SS(f.nups));
		CLib.printf(CLib.CharPtr.toCharPtr("%d local%s, %d constant%s, %d function%s\n"), f.sizelocvars, SS(f.sizelocvars), f.sizek, SS(f.sizek), f.sizep, SS(f.sizep));
	}

	private static void PrintConstants(LuaObject.Proto f) {
		int i, n = f.sizek;
		CLib.printf(CLib.CharPtr.toCharPtr("constants (%d) for %p:\n"), n, CLib.VOID(f));
		for (i = 0; i < n; i++) {
			CLib.printf(CLib.CharPtr.toCharPtr("\t%d\t"), i + 1);
			PrintConstant(f, i);
			CLib.printf(CLib.CharPtr.toCharPtr("\n"));
		}
	}

	private static void PrintLocals(LuaObject.Proto f) {
		int i, n = f.sizelocvars;
		CLib.printf(CLib.CharPtr.toCharPtr("locals (%d) for %p:\n"), n, CLib.VOID(f));
		for (i = 0; i < n; i++) {
			CLib.printf(CLib.CharPtr.toCharPtr("\t%d\t%s\t%d\t%d\n"), i, LuaObject.getstr(f.locvars[i].varname), f.locvars[i].startpc + 1, f.locvars[i].endpc + 1);
		}
	}

	private static void PrintUpvalues(LuaObject.Proto f) {
		int i, n = f.sizeupvalues;
		CLib.printf(CLib.CharPtr.toCharPtr("upvalues (%d) for %p:\n"), n, CLib.VOID(f));
		if (f.upvalues == null) {
			return;
		}
		for (i = 0; i < n; i++) {
			CLib.printf(CLib.CharPtr.toCharPtr("\t%d\t%s\n"), i, LuaObject.getstr(f.upvalues[i]));
		}
	}

	public static void PrintFunction(LuaObject.Proto f, int full) {
		int i, n = f.sizep;
		PrintHeader(f);
		PrintCode(f);
		if (full != 0) {
			PrintConstants(f);
			PrintLocals(f);
			PrintUpvalues(f);
		}
		for (i = 0; i < n; i++) {
			PrintFunction(f.p[i], full);
		}
	}
}