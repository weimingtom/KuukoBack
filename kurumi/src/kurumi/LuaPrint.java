package kurumi;

//
// ** $Id: print.c,v 1.55a 2006/05/31 13:30:05 lhf Exp $
// ** print bytecodes
// ** See Copyright Notice in lua.h
// 
//using TValue = Lua.TValue;
//using Instruction = System.UInt32;

public class LuaPrint {
	public static void luaU_print(Proto f, int full) {
		PrintFunction(f, full);
	}

	///#define Sizeof(x)	((int)sizeof(x))
	///#define VOID(p)		((const void*)(p))

	public static void PrintString(TString ts) {
		LuaConf.CharPtr s = LuaObject.getstr(ts);
		int i, n = ts.getTsv().len; //uint
		LuaConf.putchar('"');
		for (i = 0; i < n; i++) {
			int c = s.get(i);
			switch (c) {
				case '"': {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\\\""));
						break;
					}
				case '\\': {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\\\\"));
						break;
					}
				case '\u0007': { //'\a': //FIXME:
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\\a"));
						break;
					}
				case '\b': {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\\b"));
						break;
					}
				case '\f': {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\\f"));
						break;
					}
				case '\n': {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\\n"));
						break;
					}
				case '\r': {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\\r"));
						break;
					}
				case '\t': {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\\t"));
						break;
					}
				case '\u000B': { //case '\v': //FIXME:
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\\v"));
						break;
					}
				default: {
						if (LuaConf.isprint((byte)c)) {
							LuaConf.putchar(c);
						}
						else {
							LuaConf.printf(LuaConf.CharPtr.toCharPtr("\\%03u"), (byte)c);
						}
						break;
					}
			}
		}
		LuaConf.putchar('"');
	}

	private static void PrintConstant(Proto f, int i) {
		//const
 TValue o = f.k[i];
		switch (LuaObject.ttype(o)) {
		case Lua.LUA_TNIL: {
				LuaConf.printf(LuaConf.CharPtr.toCharPtr("nil"));
				break;
			}
		case Lua.LUA_TBOOLEAN: {
				LuaConf.printf(LuaObject.bvalue(o) != 0 ? LuaConf.CharPtr.toCharPtr("true") : LuaConf.CharPtr.toCharPtr("false"));
				break;
			}
		case Lua.LUA_TNUMBER: {
				LuaConf.printf(LuaConf.CharPtr.toCharPtr(LuaConf.LUA_NUMBER_FMT), LuaObject.nvalue(o));
				break;
			}
		case Lua.LUA_TSTRING: {
				PrintString(LuaObject.rawtsvalue(o));
				break;
			}
		default: {
				// cannot happen 
				LuaConf.printf(LuaConf.CharPtr.toCharPtr("? type=%d"), LuaObject.ttype(o));
				break;
			}
		}
	}

	private static void PrintCode(Proto f) {
		long[] code = f.code; //Instruction[] - UInt32[]
		int pc, n = f.sizecode;
		for (pc = 0; pc < n; pc++) {
			long i = f.code[pc]; //Instruction - UInt32
			OpCode o = LuaOpCodes.GET_OPCODE(i);
			int a = LuaOpCodes.GETARG_A(i);
			int b = LuaOpCodes.GETARG_B(i);
			int c = LuaOpCodes.GETARG_C(i);
			int bx = LuaOpCodes.GETARG_Bx(i);
			int sbx = LuaOpCodes.GETARG_sBx(i);
			int line = LuaDebug.getline(f, pc);
			LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t%d\t"), pc + 1);
			if (line > 0) {
				LuaConf.printf(LuaConf.CharPtr.toCharPtr("[%d]\t"), line);
			}
			else {
				LuaConf.printf(LuaConf.CharPtr.toCharPtr("[-]\t"));
			}
			LuaConf.printf(LuaConf.CharPtr.toCharPtr("%-9s\t"), LuaOpCodes.luaP_opnames[o.getValue()]);
			switch (LuaOpCodes.getOpMode(o)) {
				case iABC: {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("%d"), a);
						if (LuaOpCodes.getBMode(o) != OpArgMask.OpArgN) {
							LuaConf.printf(LuaConf.CharPtr.toCharPtr(" %d"), (LuaOpCodes.ISK(b) != 0) ? (-1 - LuaOpCodes.INDEXK(b)) : b);
						}
						if (LuaOpCodes.getCMode(o) != OpArgMask.OpArgN) {
							LuaConf.printf(LuaConf.CharPtr.toCharPtr(" %d"), (LuaOpCodes.ISK(c) != 0) ? (-1 - LuaOpCodes.INDEXK(c)) : c);
						}
						break;
					}
				case iABx: {
						if (LuaOpCodes.getBMode(o) == OpArgMask.OpArgK) {
							LuaConf.printf(LuaConf.CharPtr.toCharPtr("%d %d"), a, -1 - bx);
						}
						else {
							LuaConf.printf(LuaConf.CharPtr.toCharPtr("%d %d"), a, bx);
						}
						break;
					}
				case iAsBx:
					if (o == OpCode.OP_JMP) {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("%d"), sbx);
					}
					else {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("%d %d"), a, sbx);
					}
					break;
			}
			switch (o) {
				case OP_LOADK: {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t; "));
						PrintConstant(f, bx);
						break;
					}
				case OP_GETUPVAL:
				case OP_SETUPVAL: {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t; %s"), (f.sizeupvalues > 0) ? LuaObject.getstr(f.upvalues[b]) : LuaConf.CharPtr.toCharPtr("-"));
						break;
					}
				case OP_GETGLOBAL:
				case OP_SETGLOBAL: {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t; %s"), LuaObject.svalue(f.k[bx]));
						break;
					}
				case OP_GETTABLE:
				case OP_SELF: {
						if (LuaOpCodes.ISK(c) != 0) {
							LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t; "));
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
							LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t; "));
							if (LuaOpCodes.ISK(b) != 0) {
								PrintConstant(f, LuaOpCodes.INDEXK(b));
							}
							else {
								LuaConf.printf(LuaConf.CharPtr.toCharPtr("-"));
							}
							LuaConf.printf(LuaConf.CharPtr.toCharPtr(" "));
							if (LuaOpCodes.ISK(c) != 0) {
								PrintConstant(f, LuaOpCodes.INDEXK(c));
							}
							else {
								LuaConf.printf(LuaConf.CharPtr.toCharPtr("-"));
							}
						}
						break;
					}
				case OP_JMP:
				case OP_FORLOOP:
				case OP_FORPREP: {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t; to %d"), sbx + pc + 2);
						break;
					}
				case OP_CLOSURE: {
						LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t; %p"), LuaConf.VOID(f.p[bx]));
						break;
					}
				case OP_SETLIST: {
						if (c == 0) {
							LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t; %d"), (int)code[++pc]);
						}
						else {
							LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t; %d"), c);
						}
						break;
					}
				default: {
						break;
					}
			}
			LuaConf.printf(LuaConf.CharPtr.toCharPtr("\n"));
		}
	}

	public static String SS(int x) {
		return (x == 1) ? "" : "s";
	}

	///#define S(x)	x,SS(x)

	private static void PrintHeader(Proto f) {
		LuaConf.CharPtr s = LuaObject.getstr(f.source);
		if (s.get(0) == '@' || s.get(0) == '=') {
			s = s.next();
		}
		else if (s.get(0) == Lua.LUA_SIGNATURE.charAt(0)) {
			s = LuaConf.CharPtr.toCharPtr("(bstring)");
		}
		else {
			s = LuaConf.CharPtr.toCharPtr("(string)");
		}
		LuaConf.printf(LuaConf.CharPtr.toCharPtr("\n%s <%s:%d,%d> (%d Instruction%s, %d bytes at %p)\n"), (f.linedefined == 0) ? "main" : "function", s, f.linedefined, f.lastlinedefined, f.sizecode, SS(f.sizecode), f.sizecode * LuaConf.GetUnmanagedSize(new ClassType(ClassType.TYPE_LONG)), LuaConf.VOID(f));
		//typeof(long/*UInt32*//*Instruction*/)
		LuaConf.printf(LuaConf.CharPtr.toCharPtr("%d%s param%s, %d slot%s, %d upvalue%s, "), f.numparams, (f.is_vararg != 0) ? "+" : "", SS(f.numparams), f.maxstacksize, SS(f.maxstacksize), f.nups, SS(f.nups));
		LuaConf.printf(LuaConf.CharPtr.toCharPtr("%d local%s, %d constant%s, %d function%s\n"), f.sizelocvars, SS(f.sizelocvars), f.sizek, SS(f.sizek), f.sizep, SS(f.sizep));
	}

	private static void PrintConstants(Proto f) {
		int i, n = f.sizek;
		LuaConf.printf(LuaConf.CharPtr.toCharPtr("constants (%d) for %p:\n"), n, LuaConf.VOID(f));
		for (i = 0; i < n; i++) {
			LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t%d\t"), i + 1);
			PrintConstant(f, i);
			LuaConf.printf(LuaConf.CharPtr.toCharPtr("\n"));
		}
	}

	private static void PrintLocals(Proto f) {
		int i, n = f.sizelocvars;
		LuaConf.printf(LuaConf.CharPtr.toCharPtr("locals (%d) for %p:\n"), n, LuaConf.VOID(f));
		for (i = 0; i < n; i++) {
			LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t%d\t%s\t%d\t%d\n"), i, LuaObject.getstr(f.locvars[i].varname), f.locvars[i].startpc + 1, f.locvars[i].endpc + 1);
		}
	}

	private static void PrintUpvalues(Proto f) {
		int i, n = f.sizeupvalues;
		LuaConf.printf(LuaConf.CharPtr.toCharPtr("upvalues (%d) for %p:\n"), n, LuaConf.VOID(f));
		if (f.upvalues == null) {
			return;
		}
		for (i = 0; i < n; i++) {
			LuaConf.printf(LuaConf.CharPtr.toCharPtr("\t%d\t%s\n"), i, LuaObject.getstr(f.upvalues[i]));
		}
	}

	public static void PrintFunction(Proto f, int full) {
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