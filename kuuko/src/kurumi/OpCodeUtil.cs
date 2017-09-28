using System;
using System.Collections.Generic;
using System.Text;

namespace kurumi
{
    public class OpCodeUtil
    {
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
    }
}
