/*
 ** $Id: lvm.c,v 2.63.1.3 2007/12/28 15:32:23 roberto Exp $
 ** Lua virtual machine
 ** See Copyright Notice in lua.h
 */
using System;

namespace kurumi
{
	//using TValue = Lua.TValue;
	//using StkId = TValue;
	//using lua_Number = System.Double;
	//using lu_byte = System.Byte;
	//using ptrdiff_t = System.Int32;
	//using Instruction = System.UInt32;

	public class LuaVM
	{
		public static int tostring(LuaState.lua_State L, LuaObject.TValue/*StkId*/ o)
		{
			return ((LuaObject.ttype(o) == Lua.LUA_TSTRING) || (luaV_tostring(L, o) != 0)) ? 1 : 0;
		}

		public static int tonumber(/*ref*/ LuaObject.TValue[]/*StkId*/ o, LuaObject.TValue n)
		{
			return ((LuaObject.ttype(o[0]) == Lua.LUA_TNUMBER || (((o[0]) = luaV_tonumber(o[0], n)) != null))) ? 1 : 0;
		}

		public static int equalobj(LuaState.lua_State L, LuaObject.TValue o1, LuaObject.TValue o2) 
		{
			return ((LuaObject.ttype(o1) == LuaObject.ttype(o2)) && (luaV_equalval(L, o1, o2) != 0)) ? 1 : 0;
		}

		/* limit for table tag-method chains (to avoid loops) */
		public const int MAXTAGLOOP	= 100;

		public static LuaObject.TValue luaV_tonumber(LuaObject.TValue obj, LuaObject.TValue n) 
		{
			Double[]/*lua_Number*/ num = new Double[1];
			if (LuaObject.ttisnumber(obj)) 
			{
				return obj;
			}
			if (LuaObject.ttisstring(obj) && (LuaObject.luaO_str2d(LuaObject.svalue(obj), /*out*/ num) != 0))
			{
				LuaObject.setnvalue(n, num[0]);
				return n;
			}
			else
			{
				return null;
			}
		}


		public static int luaV_tostring(LuaState.lua_State L, LuaObject.TValue/*StkId*/ obj)
		{
			if (!LuaObject.ttisnumber(obj))
			{
				return 0;
			}
			else 
			{
				Double/*lua_Number*/ n = LuaObject.nvalue(obj);
				CLib.CharPtr s = LuaConf.lua_number2str(n);
				LuaObject.setsvalue2s(L, obj, LuaString.luaS_new(L, s));
				return 1;
			}
		}


		private static void traceexec(LuaState.lua_State L, LuaCode.InstructionPtr pc) 
		{
			Byte/*lu_byte*/ mask = L.hookmask;
			LuaCode.InstructionPtr oldpc = LuaCode.InstructionPtr.Assign(L.savedpc);
			L.savedpc = LuaCode.InstructionPtr.Assign(pc);
			if (((mask & Lua.LUA_MASKCOUNT) != 0) && (L.hookcount == 0))
			{
				LuaDebug.resethookcount(L);
				LuaDo.luaD_callhook(L, Lua.LUA_HOOKCOUNT, -1);
			}
			if ((mask & Lua.LUA_MASKLINE) != 0)
			{
				LuaObject.Proto p = LuaState.ci_func(L.ci).l.p;
				int npc = LuaDebug.pcRel(pc, p);
				int newline = LuaDebug.getline(p, npc);
				/* call linehook when enter a new function, when jump back (loop),
			   or when enter a new line */
                if (npc == 0 || LuaCode.InstructionPtr.lessEqual(pc, oldpc) || newline != LuaDebug.getline(p, LuaDebug.pcRel(oldpc, p)))
				{
					LuaDo.luaD_callhook(L, Lua.LUA_HOOKLINE, newline);
				}
			}
		}

		private static void callTMres(LuaState.lua_State L, LuaObject.TValue/*StkId*/ res, LuaObject.TValue f,
			LuaObject.TValue p1, LuaObject.TValue p2) 
		{
            int/*Int32*//*ptrdiff_t*/ result = LuaDo.savestack(L, res);
			LuaObject.setobj2s(L, L.top, f);  /* push function */
			LuaObject.setobj2s(L, LuaObject.TValue.plus(L.top, 1), p1);  /* 1st argument */
			LuaObject.setobj2s(L, LuaObject.TValue.plus(L.top, 2), p2);  /* 2nd argument */
			LuaDo.luaD_checkstack(L, 3);
			L.top = LuaObject.TValue.plus(L.top, 3);
			LuaDo.luaD_call(L, LuaObject.TValue.minus(L.top, 3), 1);
			res = LuaDo.restorestack(L, result);
			LuaObject.TValue[] top = new LuaObject.TValue[1];
			top[0] = L.top;
			/*StkId*/LuaObject.TValue.dec(/*ref*/ top);
			L.top = top[0];
			LuaObject.setobjs2s(L, res, L.top);
		}

		private static void callTM(LuaState.lua_State L, LuaObject.TValue f, LuaObject.TValue p1,
			LuaObject.TValue p2, LuaObject.TValue p3) 
		{
			LuaObject.setobj2s(L, L.top, f);  /* push function */
			LuaObject.setobj2s(L, LuaObject.TValue.plus(L.top, 1), p1);  /* 1st argument */
			LuaObject.setobj2s(L, LuaObject.TValue.plus(L.top, 2), p2);  /* 2nd argument */
			LuaObject.setobj2s(L, LuaObject.TValue.plus(L.top, 3), p3);  /* 3th argument */
			LuaDo.luaD_checkstack(L, 4);
			L.top = LuaObject.TValue.plus(L.top, 4);
			LuaDo.luaD_call(L, LuaObject.TValue.minus(L.top, 4), 0);
		}


		public static void luaV_gettable(LuaState.lua_State L, LuaObject.TValue t, LuaObject.TValue key, LuaObject.TValue/*StkId*/ val)
		{
			int loop;
			for (loop = 0; loop < MAXTAGLOOP; loop++) 
			{
				LuaObject.TValue tm;
				if (LuaObject.ttistable(t))
				{  
					/* `t' is a table? */
					LuaObject.Table h = LuaObject.hvalue(t);
					LuaObject.TValue res = LuaTable.luaH_get(h, key); /* do a primitive get */
					if (!LuaObject.ttisnil(res) ||  /* result is no nil? */
					    (tm = LuaTM.fasttm(L, h.metatable, LuaTM.TMS.TM_INDEX)) == null)
					{ 
						/* or no TM? */
						LuaObject.setobj2s(L, val, res);
						return;
					}
					/* else will try the tag method */
				}
				else if (LuaObject.ttisnil(tm = LuaTM.luaT_gettmbyobj(L, t, LuaTM.TMS.TM_INDEX)))
				{
					LuaDebug.luaG_typeerror(L, t, CLib.CharPtr.toCharPtr("index"));
				}
				if (LuaObject.ttisfunction(tm))
				{
					callTMres(L, val, tm, t, key);
					return;
				}
				t = tm;  /* else repeat with `tm' */
			}
			LuaDebug.luaG_runerror(L, CLib.CharPtr.toCharPtr("loop in gettable"));
		}

		public static void luaV_settable(LuaState.lua_State L, LuaObject.TValue t, LuaObject.TValue key, LuaObject.TValue/*StkId*/ val)
		{
			int loop;

			for (loop = 0; loop < MAXTAGLOOP; loop++) 
			{
				LuaObject.TValue tm;
				if (LuaObject.ttistable(t))
				{  
					/* `t' is a table? */
					LuaObject.Table h = LuaObject.hvalue(t);
					LuaObject.TValue oldval = LuaTable.luaH_set(L, h, key); /* do a primitive set */
					if (!LuaObject.ttisnil(oldval) ||  /* result is no nil? */
					    (tm = LuaTM.fasttm(L, h.metatable, LuaTM.TMS.TM_NEWINDEX)) == null)
					{ 
						/* or no TM? */
						LuaObject.setobj2t(L, oldval, val);
						LuaGC.luaC_barriert(L, h, val);
						return;
					}
					/* else will try the tag method */
				}
				else if (LuaObject.ttisnil(tm = LuaTM.luaT_gettmbyobj(L, t, LuaTM.TMS.TM_NEWINDEX)))
				{
					LuaDebug.luaG_typeerror(L, t, CLib.CharPtr.toCharPtr("index"));
				}
				if (LuaObject.ttisfunction(tm))
				{
					callTM(L, tm, t, key, val);
					return;
				}
				t = tm;  /* else repeat with `tm' */
			}
			LuaDebug.luaG_runerror(L, CLib.CharPtr.toCharPtr("loop in settable"));
		}

		private static int call_binTM(LuaState.lua_State L, LuaObject.TValue p1, LuaObject.TValue p2,
			LuaObject.TValue/*StkId*/ res, LuaTM.TMS event_) 
		{
			LuaObject.TValue tm = LuaTM.luaT_gettmbyobj(L, p1, event_);  /* try first operand */
			if (LuaObject.ttisnil(tm))
			{
				tm = LuaTM.luaT_gettmbyobj(L, p2, event_);  /* try second operand */
			}
			if (LuaObject.ttisnil(tm)) 
			{
				return 0;
			}
			callTMres(L, res, tm, p1, p2);
			return 1;
		}

		private static LuaObject.TValue get_compTM(LuaState.lua_State L, LuaObject.Table mt1, LuaObject.Table mt2,
			LuaTM.TMS event_) 
		{
			LuaObject.TValue tm1 = LuaTM.fasttm(L, mt1, event_);
			LuaObject.TValue tm2;
			if (tm1 == null)
			{
				return null;  /* no metamethod */
			}
			if (mt1 == mt2)
			{
				return tm1;  /* same metatables => same metamethods */
			}
			tm2 = LuaTM.fasttm(L, mt2, event_);
			if (tm2 == null) 
			{
				return null;  /* no metamethod */
			}
			if (LuaObject.luaO_rawequalObj(tm1, tm2) != 0)  /* same metamethods? */
			{
				return tm1;
			}
			return null;
		}

		private static int call_orderTM(LuaState.lua_State L, LuaObject.TValue p1, LuaObject.TValue p2,
			LuaTM.TMS event_) 
		{
			LuaObject.TValue tm1 = LuaTM.luaT_gettmbyobj(L, p1, event_);
			LuaObject.TValue tm2;
			if (LuaObject.ttisnil(tm1)) 
			{
				return -1;  /* no metamethod? */
			}
			tm2 = LuaTM.luaT_gettmbyobj(L, p2, event_);
			if (LuaObject.luaO_rawequalObj(tm1, tm2) == 0)  /* different metamethods? */
			{
				return -1;
			}
			callTMres(L, L.top, tm1, p1, p2);
			return LuaObject.l_isfalse(L.top) == 0 ? 1 : 0;
		}

		private static int l_strcmp(LuaObject.TString ls, LuaObject.TString rs) 
		{
			CLib.CharPtr l = LuaObject.getstr(ls);
            int/*uint*/ ll = ls.getTsv().len;
			CLib.CharPtr r = LuaObject.getstr(rs);
            int/*uint*/ lr = rs.getTsv().len;
			for (;;) 
			{
				//int temp = strcoll(l, r);
				int temp = String.Compare(l.ToString(), r.ToString());
				if (temp != 0) 
				{
					return temp;
				}
				else 
				{  
					/* strings are equal up to a `\0' */
					int/*uint*/ len = /*(uint)*/l.ToString().Length;  /* index of first `\0' in both strings */
					if (len == lr)  /* r is finished? */
					{
						return (len == ll) ? 0 : 1;
					}
					else if (len == ll)  /* l is finished? */
					{
						return -1;  /* l is smaller than r (because r is not finished) */
					}
					/* both strings longer than `len'; go on comparing (after the `\0') */
					len++;
					l = CLib.CharPtr.plus(l, len);
					ll -= len;
					r = CLib.CharPtr.plus(r, len);
					lr -= len;
				}
			}
		}

		public static int luaV_lessthan(LuaState.lua_State L, LuaObject.TValue l, LuaObject.TValue r) 
		{
			int res;
			if (LuaObject.ttype(l) != LuaObject.ttype(r))
			{
				return LuaDebug.luaG_ordererror(L, l, r);
			}
			else if (LuaObject.ttisnumber(l))
			{
				return LuaConf.luai_numlt(LuaObject.nvalue(l), LuaObject.nvalue(r)) ? 1 : 0;
			}
			else if (LuaObject.ttisstring(l))
			{
				return (l_strcmp(LuaObject.rawtsvalue(l), LuaObject.rawtsvalue(r)) < 0) ? 1 : 0;
			}
			else if ((res = call_orderTM(L, l, r, LuaTM.TMS.TM_LT)) != -1)
			{
				return res;
			}
			return LuaDebug.luaG_ordererror(L, l, r);
		}

		private static int lessequal(LuaState.lua_State L, LuaObject.TValue l, LuaObject.TValue r) 
		{
			int res;
			if (LuaObject.ttype(l) != LuaObject.ttype(r))
			{
				return LuaDebug.luaG_ordererror(L, l, r);
			}
			else if (LuaObject.ttisnumber(l))
			{
				return LuaConf.luai_numle(LuaObject.nvalue(l), LuaObject.nvalue(r)) ? 1 : 0;
				
			}
			else if (LuaObject.ttisstring(l))
			{
				return (l_strcmp(LuaObject.rawtsvalue(l), LuaObject.rawtsvalue(r)) <= 0) ? 1 : 0;
			}
			else if ((res = call_orderTM(L, l, r, LuaTM.TMS.TM_LE)) != -1)  /* first try `le' */
			{
				return res;
			}
			else if ((res = call_orderTM(L, r, l, LuaTM.TMS.TM_LT)) != -1)  /* else try `lt' */
			{
				return (res == 0) ? 1 : 0;
			}
			return LuaDebug.luaG_ordererror(L, l, r);
		}

		static CLib.CharPtr mybuff = null;

		public static int luaV_equalval(LuaState.lua_State L, LuaObject.TValue t1, LuaObject.TValue t2) 
		{
			LuaObject.TValue tm = null;
			LuaLimits.lua_assert(LuaObject.ttype(t1) == LuaObject.ttype(t2));
			switch (LuaObject.ttype(t1))
			{
				case Lua.LUA_TNIL: 
					{
						return 1;
					}
				case Lua.LUA_TNUMBER: 
					{
						return LuaConf.luai_numeq(LuaObject.nvalue(t1), LuaObject.nvalue(t2)) ? 1 : 0;
					}
				case Lua.LUA_TBOOLEAN: 
					{
						return (LuaObject.bvalue(t1) == LuaObject.bvalue(t2)) ? 1 : 0;  /* true must be 1 !! */
					}
				case Lua.LUA_TLIGHTUSERDATA: 
					{
						return (LuaObject.pvalue(t1) == LuaObject.pvalue(t2)) ? 1 : 0;
					}
				case Lua.LUA_TUSERDATA:
					{
						if (LuaObject.uvalue(t1) == LuaObject.uvalue(t2)) 
						{
							return 1;
						}
						tm = get_compTM(L, LuaObject.uvalue(t1).metatable, LuaObject.uvalue(t2).metatable,
							LuaTM.TMS.TM_EQ);
						break;  /* will try TM */
					}
				case Lua.LUA_TTABLE:
					{
						if (LuaObject.hvalue(t1) == LuaObject.hvalue(t2)) 
						{
							return 1;
						}
						tm = get_compTM(L, LuaObject.hvalue(t1).metatable, LuaObject.hvalue(t2).metatable, LuaTM.TMS.TM_EQ);
						break;  /* will try TM */
					}
				default: 
					{
						return (LuaObject.gcvalue(t1) == LuaObject.gcvalue(t2)) ? 1 : 0;
					}
			}
			if (tm == null) 
			{
				return 0;  /* no TM? */
			}
			callTMres(L, L.top, tm, t1, t2);  /* call TM */
			return LuaObject.l_isfalse(L.top) == 0 ? 1 : 0;
		}

		public static void luaV_concat (LuaState.lua_State L, int total, int last) 
		{
			do 
			{
				LuaObject.TValue/*StkId*/ top = LuaObject.TValue.plus(L.base_, last + 1);
				int n = 2;  /* number of elements handled in this pass (at least 2) */
				if (!(LuaObject.ttisstring(LuaObject.TValue.minus(top, 2)) || LuaObject.ttisnumber(LuaObject.TValue.minus(top, 2))) || (tostring(L, LuaObject.TValue.minus(top, 1)) == 0))
				{
					if (call_binTM(L, LuaObject.TValue.minus(top, 2), LuaObject.TValue.minus(top, 1), LuaObject.TValue.minus(top, 2), LuaTM.TMS.TM_CONCAT) == 0)
					{
						LuaDebug.luaG_concaterror(L, LuaObject.TValue.minus(top, 2), LuaObject.TValue.minus(top, 1));
					}
				}
				else if (LuaObject.tsvalue(LuaObject.TValue.minus(top, 1)).len == 0)  /* second op is empty? */
				{
					tostring(L, LuaObject.TValue.minus(top, 2));  /* result is first op (as string) */
				}
				else 
				{
					/* at least two string values; get as many as possible */
					int/*uint*/ tl = LuaObject.tsvalue(LuaObject.TValue.minus(top, 1)).len;
					CLib.CharPtr buffer;
					int i;
					/* collect total length */
					for (n = 1; n < total && (tostring(L, LuaObject.TValue.minus(LuaObject.TValue.minus(top, n), 1)) != 0); n++) //FIXME:
					{
						int/*uint*/ l = LuaObject.tsvalue(LuaObject.TValue.minus(LuaObject.TValue.minus(top, n), 1)).len;
						if (l >= LuaLimits.MAX_SIZET - tl) 
						{
							LuaDebug.luaG_runerror(L, CLib.CharPtr.toCharPtr("string length overflow"));
						}
						tl += l;
					}
					buffer = LuaZIO.luaZ_openspace(L, LuaState.G(L).buff, tl);
					if (CLib.CharPtr.isEqual(mybuff, null))
					{
						mybuff = buffer;
					}
					tl = 0;
					for (i = n; i > 0; i--) 
					{  
						/* concat all strings */
						int/*uint*/ l = LuaObject.tsvalue(LuaObject.TValue.minus(top, i)).len;
						CLib.memcpy_char(buffer.chars, /*(int)*/tl, LuaObject.svalue(LuaObject.TValue.minus(top, i)).chars, (int)l);
						tl += l;
					}
					LuaObject.setsvalue2s(L, LuaObject.TValue.minus(top, n), LuaString.luaS_newlstr(L, buffer, tl));
				}
				total -= n - 1;  /* got `n' strings to create 1 new */
				last -= n - 1;
			} while (total > 1);  /* repeat until only 1 result left */
		}

		public static void Arith(LuaState.lua_State L, LuaObject.TValue/*StkId*/ ra, LuaObject.TValue rb,
			LuaObject.TValue rc, LuaTM.TMS op) 
		{
			LuaObject.TValue tempb = new LuaObject.TValue(), tempc = new LuaObject.TValue();
			LuaObject.TValue b, c;
			if ((b = luaV_tonumber(rb, tempb)) != null &&
			    (c = luaV_tonumber(rc, tempc)) != null) 
			{
				Double/*lua_Number*/ nb = LuaObject.nvalue(b), nc = LuaObject.nvalue(c);
				switch (op) 
				{
					case LuaTM.TMS.TM_ADD: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_numadd(nb, nc)); 
							break;
						}
					case LuaTM.TMS.TM_SUB: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_numsub(nb, nc)); 
							break;
						}
					case LuaTM.TMS.TM_MUL: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_nummul(nb, nc)); 
							break;
						}
					case LuaTM.TMS.TM_DIV: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_numdiv(nb, nc)); 
							break;
						}
					case LuaTM.TMS.TM_MOD: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_nummod(nb, nc)); 
							break;
						}
					case LuaTM.TMS.TM_POW: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_numpow(nb, nc)); 
							break;
						}
					case LuaTM.TMS.TM_UNM: 
						{
							LuaObject.setnvalue(ra, LuaConf.luai_numunm(nb)); 
							break;
						}
					default: 
						{
							LuaLimits.lua_assert(false); 
							break;
						}
				}
			}
			else if (call_binTM(L, rb, rc, ra, op) == 0)
			{
				LuaDebug.luaG_aritherror(L, rb, rc);
			}
		}

		/*
		 ** some macros for common tasks in `luaV_execute'
		 */
		public static void runtime_check(LuaState.lua_State L, bool c)	
		{ 
			ClassType.Assert(c); 
		}

		//#define RA(i)	(base+GETARG_A(i))
		/* to be used after possible stack reallocation */
		//#define RB(i)	check_exp(getBMode(GET_OPCODE(i)) == OpArgMask.OpArgR, base+GETARG_B(i))
		//#define RC(i)	check_exp(getCMode(GET_OPCODE(i)) == OpArgMask.OpArgR, base+GETARG_C(i))
		//#define RKB(i)	check_exp(getBMode(GET_OPCODE(i)) == OpArgMask.OpArgK, \
		//ISK(GETARG_B(i)) ? k+INDEXK(GETARG_B(i)) : base+GETARG_B(i))
		//#define RKC(i)	check_exp(getCMode(GET_OPCODE(i)) == OpArgMask.OpArgK, \
		//	ISK(GETARG_C(i)) ? k+INDEXK(GETARG_C(i)) : base+GETARG_C(i))
		//#define KBx(i)	check_exp(getBMode(GET_OPCODE(i)) == OpArgMask.OpArgK, k+GETARG_Bx(i))

		// todo: implement proper checks, as above
		public static LuaObject.TValue RA(LuaState.lua_State L, LuaObject.TValue/*StkId*/ base_, long/*UInt32*//*Instruction*/ i) 
		{ 
			return LuaObject.TValue.plus(base_, LuaOpCodes.GETARG_A(i)); 
		}
		
		public static LuaObject.TValue RB(LuaState.lua_State L, LuaObject.TValue/*StkId*/ base_, long/*UInt32*//*Instruction*/ i) 
		{ 
			return LuaObject.TValue.plus(base_, LuaOpCodes.GETARG_B(i)); 
		}
		
		public static LuaObject.TValue RC(LuaState.lua_State L, LuaObject.TValue/*StkId*/ base_, long/*UInt32*//*Instruction*/ i) 
		{ 
			return LuaObject.TValue.plus(base_, LuaOpCodes.GETARG_C(i)); 
		}
		
		public static LuaObject.TValue RKB(LuaState.lua_State L, LuaObject.TValue/*StkId*/ base_, long/*UInt32*//*Instruction*/ i, LuaObject.TValue[] k) 
		{ 
			return LuaOpCodes.ISK(LuaOpCodes.GETARG_B(i)) != 0 ? k[LuaOpCodes.INDEXK(LuaOpCodes.GETARG_B(i))] : LuaObject.TValue.plus(base_, LuaOpCodes.GETARG_B(i)); 
		}
		
		public static LuaObject.TValue RKC(LuaState.lua_State L, LuaObject.TValue/*StkId*/ base_, long/*UInt32*//*Instruction*/ i, LuaObject.TValue[] k) 
		{ 
			return LuaOpCodes.ISK(LuaOpCodes.GETARG_C(i)) != 0 ? k[LuaOpCodes.INDEXK(LuaOpCodes.GETARG_C(i))] : LuaObject.TValue.plus(base_, LuaOpCodes.GETARG_C(i)); 
		}
		
		public static LuaObject.TValue KBx(LuaState.lua_State L, long/*UInt32*//*Instruction*/ i, LuaObject.TValue[] k) 
		{ 
			return k[LuaOpCodes.GETARG_Bx(i)]; 
		}

		public static void dojump(LuaState.lua_State L, LuaCode.InstructionPtr pc, int i) 
		{ 
			pc.pc += i; 
			LuaLimits.luai_threadyield(L);
		}

		//#define Protect(x)	{ L.savedpc = pc; {x;}; base = L.base_; }

		public static void arith_op(LuaState.lua_State L, LuaConf.op_delegate op, LuaTM.TMS tm, LuaObject.TValue/*StkId*/ base_, long/*UInt32*//*Instruction*/ i, LuaObject.TValue[] k, LuaObject.TValue/*StkId*/ ra, LuaCode.InstructionPtr pc)
		{
			LuaObject.TValue rb = RKB(L, base_, i, k);
			LuaObject.TValue rc = RKC(L, base_, i, k);
			if (LuaObject.ttisnumber(rb) && LuaObject.ttisnumber(rc))
			{
				Double/*lua_Number*/ nb = LuaObject.nvalue(rb), nc = LuaObject.nvalue(rc);
				LuaObject.setnvalue(ra, op.exec(nb, nc));
			}
			else
			{
				//Protect(
				L.savedpc = LuaCode.InstructionPtr.Assign(pc);
				Arith(L, ra, rb, rc, tm);
				base_ = L.base_;
				//);
			}
		}

		private static void Dump(int pc, long/*UInt32*//*Instruction*/ i)
		{
			int A = LuaOpCodes.GETARG_A(i);
			int B = LuaOpCodes.GETARG_B(i);
			int C = LuaOpCodes.GETARG_C(i);
			int Bx = LuaOpCodes.GETARG_Bx(i);
			int sBx = LuaOpCodes.GETARG_sBx(i);
			if ((sBx & 0x100) != 0)
			{
				sBx = - (sBx & 0xff);
			}
            StreamProxy.Write("" + pc + " (" + i + "): "); //FIXME:"{0,5} ({1,10}): "
            StreamProxy.Write("" + LuaOpCodes.luaP_opnames[(int)LuaOpCodes.GET_OPCODE(i)].ToString() + "\t"); //"{0,-10}\t"
			switch (LuaOpCodes.GET_OPCODE(i))
			{
				case LuaOpCodes.OpCode.OP_CLOSE:
					{
                        StreamProxy.Write("" + A + "");
						break;
					}
				case LuaOpCodes.OpCode.OP_MOVE:
				case LuaOpCodes.OpCode.OP_LOADNIL:
				case LuaOpCodes.OpCode.OP_GETUPVAL:
				case LuaOpCodes.OpCode.OP_SETUPVAL:
				case LuaOpCodes.OpCode.OP_UNM:
				case LuaOpCodes.OpCode.OP_NOT:
				case LuaOpCodes.OpCode.OP_RETURN:
					{
                        StreamProxy.Write("" + A + ", " + B + "");
						break;
					}
				case LuaOpCodes.OpCode.OP_LOADBOOL:
				case LuaOpCodes.OpCode.OP_GETTABLE:
				case LuaOpCodes.OpCode.OP_SETTABLE:
				case LuaOpCodes.OpCode.OP_NEWTABLE:
				case LuaOpCodes.OpCode.OP_SELF:
				case LuaOpCodes.OpCode.OP_ADD:
				case LuaOpCodes.OpCode.OP_SUB:
				case LuaOpCodes.OpCode.OP_MUL:
				case LuaOpCodes.OpCode.OP_DIV:
				case LuaOpCodes.OpCode.OP_POW:
				case LuaOpCodes.OpCode.OP_CONCAT:
				case LuaOpCodes.OpCode.OP_EQ:
				case LuaOpCodes.OpCode.OP_LT:
				case LuaOpCodes.OpCode.OP_LE:
				case LuaOpCodes.OpCode.OP_TEST:
				case LuaOpCodes.OpCode.OP_CALL:
				case LuaOpCodes.OpCode.OP_TAILCALL:
					{
                        StreamProxy.Write("" + A + ", " + B + ", " + C + "");
						break;
					}
				case LuaOpCodes.OpCode.OP_LOADK:
					{
                        StreamProxy.Write("" + A + ", " + Bx + "");
						break;
					}
				case LuaOpCodes.OpCode.OP_GETGLOBAL:
				case LuaOpCodes.OpCode.OP_SETGLOBAL:
				case LuaOpCodes.OpCode.OP_SETLIST:
				case LuaOpCodes.OpCode.OP_CLOSURE:
					{
                        StreamProxy.Write("" + A + ", " + Bx + "");
						break;
					}
				case LuaOpCodes.OpCode.OP_TFORLOOP:
					{
                        StreamProxy.Write("" + A + ", " + C + "");
						break;
					}
				case LuaOpCodes.OpCode.OP_JMP:
				case LuaOpCodes.OpCode.OP_FORLOOP:
				case LuaOpCodes.OpCode.OP_FORPREP:
					{
                        StreamProxy.Write("" + A + ", " + sBx + "");
						break;
					}
			}
            StreamProxy.WriteLine();

		}

		public static void luaV_execute(LuaState.lua_State L, int nexeccalls) 
		{
			LuaObject.LClosure cl;
			LuaObject.TValue/*StkId*/ base_;
			LuaObject.TValue[] k;
			/*const*/ LuaCode.InstructionPtr pc;
			//reentry:  /* entry point */
			while (true)
			{
				bool reentry = false;
				
				LuaLimits.lua_assert(LuaState.isLua(L.ci));
				pc = LuaCode.InstructionPtr.Assign(L.savedpc);
				cl = LuaObject.clvalue(L.ci.func).l;
				base_ = L.base_;
				k = cl.p.k;
				/* main loop of interpreter */
				for (;;) 
				{
					LuaCode.InstructionPtr[] pc_ref = new LuaCode.InstructionPtr[1];
					pc_ref[0] = pc;
					LuaCode.InstructionPtr ret = LuaCode.InstructionPtr.inc(/*ref*/ pc_ref);
					pc = pc_ref[0];
					/*const*/ long/*UInt32*//*Instruction*/ i = ret.get(0);
					LuaObject.TValue/*StkId*/ ra;
					if (((L.hookmask & (Lua.LUA_MASKLINE | Lua.LUA_MASKCOUNT)) != 0) &&
					    (((--L.hookcount) == 0) || ((L.hookmask & Lua.LUA_MASKLINE) != 0)))
					{
						traceexec(L, pc);
						if (L.status == Lua.LUA_YIELD)
						{  
							/* did hook yield? */
							L.savedpc = new LuaCode.InstructionPtr(pc.codes, pc.pc - 1);
							return;
						}
						base_ = L.base_;
					}
					/* warning!! several calls may realloc the stack and invalidate `ra' */
					ra = RA(L, base_, i);
					LuaLimits.lua_assert(base_ == L.base_ && L.base_ == L.ci.base_);
					LuaLimits.lua_assert(LuaObject.TValue.lessEqual(base_, L.top) && ((LuaObject.TValue.minus(L.top, L.stack)) <= L.stacksize));
					LuaLimits.lua_assert(L.top == L.ci.top || (LuaDebug.luaG_checkopenop(i) != 0));
					//Dump(pc.pc, i);
					
					bool reentry2 = false;
					
					switch (LuaOpCodes.GET_OPCODE(i))
					{
						case LuaOpCodes.OpCode.OP_MOVE: 
							{
								LuaObject.setobjs2s(L, ra, RB(L, base_, i));
								continue;
							}
						case LuaOpCodes.OpCode.OP_LOADK: 
							{
								LuaObject.setobj2s(L, ra, KBx(L, i, k));
								continue;
							}
						case LuaOpCodes.OpCode.OP_LOADBOOL: 
							{
								LuaObject.setbvalue(ra, LuaOpCodes.GETARG_B(i));
								if (LuaOpCodes.GETARG_C(i) != 0) 
								{
									LuaCode.InstructionPtr[] pc_ref2 = new LuaCode.InstructionPtr[1];
									pc_ref2[0] = pc;
									LuaCode.InstructionPtr.inc(/*ref*/ pc_ref2);  /* skip next instruction (if C) */
									pc = pc_ref2[0];
								}
								continue;
							}
						case LuaOpCodes.OpCode.OP_LOADNIL: 
							{
								LuaObject.TValue rb = RB(L, base_, i);
								do 
								{
									LuaObject.TValue[] rb_ref = new LuaObject.TValue[1];
									rb_ref[0] = rb;
									LuaObject.TValue ret2 = /*StkId*/LuaObject.TValue.dec(/*ref*/ rb_ref);
									rb = rb_ref[0];
									LuaObject.setnilvalue(ret2);
								} while (LuaObject.TValue.greaterEqual(rb, ra));
								continue;
							}
						case LuaOpCodes.OpCode.OP_GETUPVAL: 
							{
								int b = LuaOpCodes.GETARG_B(i);
								LuaObject.setobj2s(L, ra, cl.upvals[b].v);
								continue;
							}
						case LuaOpCodes.OpCode.OP_GETGLOBAL: 
							{
								LuaObject.TValue g = new LuaObject.TValue();
								LuaObject.TValue rb = KBx(L, i, k);
								LuaObject.sethvalue(L, g, cl.getEnv());
								LuaLimits.lua_assert(LuaObject.ttisstring(rb));
								//Protect(
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								luaV_gettable(L, g, rb, ra);
								base_ = L.base_;
								//);
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								continue;
							}
						case LuaOpCodes.OpCode.OP_GETTABLE: 
							{
								//Protect(
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								luaV_gettable(L, RB(L, base_, i), RKC(L, base_, i, k), ra);
								base_ = L.base_;
								//);
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								continue;
							}
						case LuaOpCodes.OpCode.OP_SETGLOBAL: 
							{
								LuaObject.TValue g = new LuaObject.TValue();
								LuaObject.sethvalue(L, g, cl.getEnv());
								LuaLimits.lua_assert(LuaObject.ttisstring(KBx(L, i, k)));
								//Protect(
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								luaV_settable(L, g, KBx(L, i, k), ra);
								base_ = L.base_;
								//);
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								continue;
							}
						case LuaOpCodes.OpCode.OP_SETUPVAL: 
							{
								LuaObject.UpVal uv = cl.upvals[LuaOpCodes.GETARG_B(i)];
								LuaObject.setobj(L, uv.v, ra);
								LuaGC.luaC_barrier(L, uv, ra);
								continue;
							}
						case LuaOpCodes.OpCode.OP_SETTABLE: 
							{
								//Protect(
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								luaV_settable(L, ra, RKB(L, base_, i, k), RKC(L, base_, i, k));
								base_ = L.base_;
								//);
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								continue;
							}
						case LuaOpCodes.OpCode.OP_NEWTABLE: 
							{
								int b = LuaOpCodes.GETARG_B(i);
								int c = LuaOpCodes.GETARG_C(i);
								LuaObject.sethvalue(L, ra, LuaTable.luaH_new(L, LuaObject.luaO_fb2int(b), LuaObject.luaO_fb2int(c)));
								//Protect(
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								LuaGC.luaC_checkGC(L);
								base_ = L.base_;
								//);
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								continue;
							}
						case LuaOpCodes.OpCode.OP_SELF: 
							{
								/*StkId*/LuaObject.TValue rb = RB(L, base_, i);
								LuaObject.setobjs2s(L, LuaObject.TValue.plus(ra, 1), rb);
								//Protect(
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								luaV_gettable(L, rb, RKC(L, base_, i, k), ra);
								base_ = L.base_;
								//);
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								continue;
							}
						case LuaOpCodes.OpCode.OP_ADD: 
							{
								arith_op(L, new LuaConf.luai_numadd_delegate(), LuaTM.TMS.TM_ADD, base_, i, k, ra, pc);
								continue;
							}
						case LuaOpCodes.OpCode.OP_SUB: 
							{
								arith_op(L, new LuaConf.luai_numsub_delegate(), LuaTM.TMS.TM_SUB, base_, i, k, ra, pc);
								continue;
							}
						case LuaOpCodes.OpCode.OP_MUL: 
							{
								arith_op(L, new LuaConf.luai_nummul_delegate(), LuaTM.TMS.TM_MUL, base_, i, k, ra, pc);
								continue;
							}
						case LuaOpCodes.OpCode.OP_DIV: 
							{
								arith_op(L, new LuaConf.luai_numdiv_delegate(), LuaTM.TMS.TM_DIV, base_, i, k, ra, pc);
								continue;
							}
						case LuaOpCodes.OpCode.OP_MOD: 
							{
								arith_op(L, new LuaConf.luai_nummod_delegate(), LuaTM.TMS.TM_MOD, base_, i, k, ra, pc);
								continue;
							}
						case LuaOpCodes.OpCode.OP_POW: 
							{
								arith_op(L, new LuaConf.luai_numpow_delegate(), LuaTM.TMS.TM_POW, base_, i, k, ra, pc);
								continue;
							}
						case LuaOpCodes.OpCode.OP_UNM: 
							{
								LuaObject.TValue rb = RB(L, base_, i);
								if (LuaObject.ttisnumber(rb))
								{
									Double/*lua_Number*/ nb = LuaObject.nvalue(rb);
									LuaObject.setnvalue(ra, LuaConf.luai_numunm(nb));
								}
								else 
								{
									//Protect(
									L.savedpc = LuaCode.InstructionPtr.Assign(pc);
									Arith(L, ra, rb, rb, LuaTM.TMS.TM_UNM);
									base_ = L.base_;
									//);
									L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								}
								continue;
							}
						case LuaOpCodes.OpCode.OP_NOT: 
							{
								int res = LuaObject.l_isfalse(RB(L, base_, i)) == 0 ? 0 : 1;  /* next assignment may change this value */
								LuaObject.setbvalue(ra, res);
								continue;
							}
						case LuaOpCodes.OpCode.OP_LEN: 
							{
								LuaObject.TValue rb = RB(L, base_, i);
								switch (LuaObject.ttype(rb))
								{
									case Lua.LUA_TTABLE:
										{
											LuaObject.setnvalue(ra, (Double/*lua_Number*/)LuaTable.luaH_getn(LuaObject.hvalue(rb)));
											break;
										}
									case Lua.LUA_TSTRING:
										{
											LuaObject.setnvalue(ra, (Double/*lua_Number*/)LuaObject.tsvalue(rb).len);
											break;
										}
									default: 
										{  
											/* try metamethod */
											//Protect(
											L.savedpc = LuaCode.InstructionPtr.Assign(pc);
											if (call_binTM(L, rb, LuaObject.luaO_nilobject, ra, LuaTM.TMS.TM_LEN) == 0)
											{
												LuaDebug.luaG_typeerror(L, rb, CLib.CharPtr.toCharPtr("get length of"));
											}
											base_ = L.base_;
											//)
											break;
										}
								}
								continue;
							}
						case LuaOpCodes.OpCode.OP_CONCAT: 
							{
								int b = LuaOpCodes.GETARG_B(i);
								int c = LuaOpCodes.GETARG_C(i);
								//Protect(
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								luaV_concat(L, c - b + 1, c); LuaGC.luaC_checkGC(L);
								base_ = L.base_;
								//);
								LuaObject.setobjs2s(L, RA(L, base_, i), LuaObject.TValue.plus(base_, b));
								continue;
							}
						case LuaOpCodes.OpCode.OP_JMP: 
							{
								dojump(L, pc, LuaOpCodes.GETARG_sBx(i));
								continue;
							}
						case LuaOpCodes.OpCode.OP_EQ: 
							{
								LuaObject.TValue rb = RKB(L, base_, i, k);
								LuaObject.TValue rc = RKC(L, base_, i, k);
								//Protect(
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								if (equalobj(L, rb, rc) == LuaOpCodes.GETARG_A(i))
									dojump(L, pc, LuaOpCodes.GETARG_sBx(pc.get(0)));
								base_ = L.base_;
								//);
								LuaCode.InstructionPtr[] pc_ref2 = new LuaCode.InstructionPtr[1];
								pc_ref2[0] = pc;
								LuaCode.InstructionPtr.inc(/*ref*/ pc_ref2);
								pc = pc_ref2[0];
								continue;
							}
						case LuaOpCodes.OpCode.OP_LT: 
							{
								//Protect(
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								if (luaV_lessthan(L, RKB(L, base_, i, k), RKC(L, base_, i, k)) == LuaOpCodes.GETARG_A(i))
								{
									dojump(L, pc, LuaOpCodes.GETARG_sBx(pc.get(0)));
								}
								base_ = L.base_;
								//);
								LuaCode.InstructionPtr[] pc_ref3 = new LuaCode.InstructionPtr[1];
								pc_ref3[0] = pc;
								LuaCode.InstructionPtr.inc(/*ref*/ pc_ref3);
								pc = pc_ref3[0];
								continue;
							}
						case LuaOpCodes.OpCode.OP_LE: 
							{
								//Protect(
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								if (lessequal(L, RKB(L, base_, i, k), RKC(L, base_, i, k)) == LuaOpCodes.GETARG_A(i))
								{
									dojump(L, pc, LuaOpCodes.GETARG_sBx(pc.get(0)));
								}
								base_ = L.base_;
								//);
								LuaCode.InstructionPtr[] pc_ref4 = new LuaCode.InstructionPtr[1];
								pc_ref4[0] = pc;
								LuaCode.InstructionPtr.inc(/*ref*/ pc_ref4);
								pc = pc_ref4[0];
								continue;
							}
						case LuaOpCodes.OpCode.OP_TEST: 
							{
								if (LuaObject.l_isfalse(ra) != LuaOpCodes.GETARG_C(i))
								{
									dojump(L, pc, LuaOpCodes.GETARG_sBx(pc.get(0)));
								}
								LuaCode.InstructionPtr[] pc_ref5 = new LuaCode.InstructionPtr[1];
								pc_ref5[0] = pc;
								LuaCode.InstructionPtr.inc(/*ref*/ pc_ref5);
								pc = pc_ref5[0];
								continue;
							}
						case LuaOpCodes.OpCode.OP_TESTSET: 
							{
								LuaObject.TValue rb = RB(L, base_, i);
								if (LuaObject.l_isfalse(rb) != LuaOpCodes.GETARG_C(i))
								{
									LuaObject.setobjs2s(L, ra, rb);
									dojump(L, pc, LuaOpCodes.GETARG_sBx(pc.get(0)));
								}
								LuaCode.InstructionPtr[] pc_ref6 = new LuaCode.InstructionPtr[1];
								pc_ref6[0] = pc;
								LuaCode.InstructionPtr.inc(/*ref*/ pc_ref6);
								pc = pc_ref6[0];
								continue;
							}
						case LuaOpCodes.OpCode.OP_CALL: 
							{
								int b = LuaOpCodes.GETARG_B(i);
								int nresults = LuaOpCodes.GETARG_C(i) - 1;
								if (b != 0) 
								{
									L.top = LuaObject.TValue.plus(ra, b);  /* else previous instruction set top */
								}
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								bool reentry3 = false;
								switch (LuaDo.luaD_precall(L, ra, nresults))
								{
									case LuaDo.PCRLUA:
										{
											nexeccalls++;
											//goto reentry;  /* restart luaV_execute over new Lua function */
											reentry3 = true;
											break;
										}
									case LuaDo.PCRC:
										{
											/* it was a C function (`precall' called it); adjust results */
											if (nresults >= 0) 
											{
												L.top = L.ci.top;
											}
											base_ = L.base_;
											continue;
										}
									default: 
										{
											return;  /* yield */
										}
								}
								if (reentry3)
								{
									reentry2 = true;
									break;
								}
								else
								{
									break;
								}
							}
						case LuaOpCodes.OpCode.OP_TAILCALL: 
							{
								int b = LuaOpCodes.GETARG_B(i);
								if (b != 0) 
								{
									L.top = LuaObject.TValue.plus(ra, b);  /* else previous instruction set top */
								}
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								LuaLimits.lua_assert(LuaOpCodes.GETARG_C(i) - 1 == Lua.LUA_MULTRET);
								bool reentry4 = false;
								switch (LuaDo.luaD_precall(L, ra, Lua.LUA_MULTRET))
								{
									case LuaDo.PCRLUA:
										{
											/* tail call: put new frame in place of previous one */
											LuaState.CallInfo ci = LuaState.CallInfo.minus(L.ci, 1);  /* previous frame */
											int aux;
											LuaObject.TValue/*StkId*/ func = ci.func;
											LuaObject.TValue/*StkId*/ pfunc = LuaState.CallInfo.plus(ci, 1).func;  /* previous function index */
											if (L.openupval != null) 
											{
												LuaFunc.luaF_close(L, ci.base_);
											}
											L.base_ = ci.base_ = LuaObject.TValue.plus(ci.func, LuaObject.TValue.minus(ci.get(1).base_, pfunc));
											for (aux = 0; LuaObject.TValue.lessThan(LuaObject.TValue.plus(pfunc, aux), L.top); aux++)  /* move frame down */
											{
												LuaObject.setobjs2s(L, LuaObject.TValue.plus(func, aux), LuaObject.TValue.plus(pfunc, aux));
											}
											ci.top = L.top = LuaObject.TValue.plus(func, aux);  /* correct top */
											LuaLimits.lua_assert(L.top == LuaObject.TValue.plus(L.base_, LuaObject.clvalue(func).l.p.maxstacksize));
											ci.savedpc = LuaCode.InstructionPtr.Assign(L.savedpc);
											ci.tailcalls++;  /* one more call lost */
											LuaState.CallInfo[] ci_ref3 = new LuaState.CallInfo[1];
											ci_ref3[0] = L.ci;
											LuaState.CallInfo.dec(/*ref*/ ci_ref3);  /* remove new frame */
											L.ci = ci_ref3[0];
											//goto reentry;
											reentry4 = true;
											break;
										}
									case LuaDo.PCRC:
										{  
											/* it was a C function (`precall' called it) */
											base_ = L.base_;
											continue;
										}
									default: 
										{
											return;  /* yield */
										}
								}
								if (reentry4)
								{
									reentry2 = true;
									break;
								}
								else
								{
									break;
								}
							}
						case LuaOpCodes.OpCode.OP_RETURN: 
							{
								int b = LuaOpCodes.GETARG_B(i);
								if (b != 0) 
								{
									L.top = LuaObject.TValue.plus(ra, b - 1); //FIXME:
								}
								if (L.openupval != null) 
								{
									LuaFunc.luaF_close(L, base_);
								}
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								b = LuaDo.luaD_poscall(L, ra);
								if (--nexeccalls == 0)  /* was previous function running `here'? */
								{
									return;  /* no: return */
								}
								else 
								{  
									/* yes: continue its execution */
									if (b != 0) 
									{
										L.top = L.ci.top;
									}
									LuaLimits.lua_assert(LuaState.isLua(L.ci));
									LuaLimits.lua_assert(LuaOpCodes.GET_OPCODE(L.ci.savedpc.get(-1)) == LuaOpCodes.OpCode.OP_CALL);
									//goto reentry;
									reentry2 = true;
									break;
								}
							}
						case LuaOpCodes.OpCode.OP_FORLOOP: 
							{
								Double/*lua_Number*/ step = LuaObject.nvalue(LuaObject.TValue.plus(ra, 2));
								Double/*lua_Number*/ idx = LuaConf.luai_numadd(LuaObject.nvalue(ra), step); /* increment index */
								Double/*lua_Number*/ limit = LuaObject.nvalue(LuaObject.TValue.plus(ra, 1));
								if (LuaConf.luai_numlt(0, step) ? LuaConf.luai_numle(idx, limit)
								    : LuaConf.luai_numle(limit, idx))
								{
									dojump(L, pc, LuaOpCodes.GETARG_sBx(i));  /* jump back */
									LuaObject.setnvalue(ra, idx);  /* update internal index... */
									LuaObject.setnvalue(LuaObject.TValue.plus(ra, 3), idx);  /* ...and external index */
								}
								continue;
							}
						case LuaOpCodes.OpCode.OP_FORPREP: 
							{
								LuaObject.TValue init = ra;
								LuaObject.TValue plimit = LuaObject.TValue.plus(ra, 1);
								LuaObject.TValue pstep = LuaObject.TValue.plus(ra, 2);
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);  /* next steps may throw errors */
								int retxxx;
								LuaObject.TValue[] init_ref = new LuaObject.TValue[1];
								init_ref[0] = init;
								retxxx = tonumber(/*ref*/ init_ref, ra);
								init = init_ref[0];
								if (retxxx == 0)
								{
									LuaDebug.luaG_runerror(L, CLib.CharPtr.toCharPtr(LuaConf.LUA_QL("for") + " initial value must be a number"));
								} 
								else 
								{
									LuaObject.TValue[] plimit_ref = new LuaObject.TValue[1];
									plimit_ref[0] = plimit;
									retxxx = tonumber(/*ref*/ plimit_ref, LuaObject.TValue.plus(ra, 1));
									plimit = plimit_ref[0];
									if (retxxx == 0)
									{
		                                LuaDebug.luaG_runerror(L, CLib.CharPtr.toCharPtr(LuaConf.LUA_QL("for") + " limit must be a number"));
									}
									else
									{
										LuaObject.TValue[] pstep_ref = new LuaObject.TValue[1];
										pstep_ref[0] = pstep;
										retxxx = tonumber(/*ref*/ pstep_ref, LuaObject.TValue.plus(ra, 2));
										pstep = pstep_ref[0];									
										if (retxxx == 0)
										{
											LuaDebug.luaG_runerror(L, CLib.CharPtr.toCharPtr(LuaConf.LUA_QL("for") + " step must be a number"));
										}
									}
								}
								LuaObject.setnvalue(ra, LuaConf.luai_numsub(LuaObject.nvalue(ra), LuaObject.nvalue(pstep)));
								dojump(L, pc, LuaOpCodes.GETARG_sBx(i));
								continue;
							}
						case LuaOpCodes.OpCode.OP_TFORLOOP: 
							{
								LuaObject.TValue/*StkId*/ cb = LuaObject.TValue.plus(ra, 3);  /* call base */
								LuaObject.setobjs2s(L, LuaObject.TValue.plus(cb, 2), LuaObject.TValue.plus(ra, 2));
								LuaObject.setobjs2s(L, LuaObject.TValue.plus(cb, 1), LuaObject.TValue.plus(ra, 1));
								LuaObject.setobjs2s(L, cb, ra);
								L.top = LuaObject.TValue.plus(cb, 3);  /* func. + 2 args (state and index) */
								//Protect(
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								LuaDo.luaD_call(L, cb, LuaOpCodes.GETARG_C(i));
								base_ = L.base_;
								//);
								L.top = L.ci.top;
								cb = LuaObject.TValue.plus(RA(L, base_, i), 3);  /* previous call may change the stack */
								if (!LuaObject.ttisnil(cb))
								{  
									/* continue loop? */
									LuaObject.setobjs2s(L, LuaObject.TValue.minus(cb, 1), cb);  /* save control variable */
									dojump(L, pc, LuaOpCodes.GETARG_sBx(pc.get(0)));  /* jump back */
								}
								LuaCode.InstructionPtr[] pc_ref3 = new LuaCode.InstructionPtr[1];
								pc_ref3[0] = pc;
								LuaCode.InstructionPtr.inc(/*ref*/ pc_ref3);
								pc = pc_ref3[0];
								continue;
							}
						case LuaOpCodes.OpCode.OP_SETLIST: 
							{
								int n = LuaOpCodes.GETARG_B(i);
								int c = LuaOpCodes.GETARG_C(i);
								int last;
								LuaObject.Table h;
								if (n == 0) 
								{
									n = LuaLimits.cast_int(LuaObject.TValue.minus(L.top, ra)) - 1;
									L.top = L.ci.top;
								}
								if (c == 0)
								{
									c = LuaLimits.cast_int_instruction(pc.get(0));
									LuaCode.InstructionPtr[] pc_ref5 = new LuaCode.InstructionPtr[1];
									pc_ref5[0] = pc;
									LuaCode.InstructionPtr.inc(/*ref*/ pc_ref5);
									pc = pc_ref5[0];
								}
								runtime_check(L, LuaObject.ttistable(ra));
								h = LuaObject.hvalue(ra);
								last = ((c - 1) * LuaOpCodes.LFIELDS_PER_FLUSH) + n;
								if (last > h.sizearray)  /* needs more space? */
								{
									LuaTable.luaH_resizearray(L, h, last);  /* pre-alloc it at once */
								}
								for (; n > 0; n--) 
								{
									LuaObject.TValue val = LuaObject.TValue.plus(ra, n);
									LuaObject.setobj2t(L, LuaTable.luaH_setnum(L, h, last--), val);
									LuaGC.luaC_barriert(L, h, val);
								}
								continue;
							}
						case LuaOpCodes.OpCode.OP_CLOSE: 
							{
								LuaFunc.luaF_close(L, ra);
								continue;
							}
						case LuaOpCodes.OpCode.OP_CLOSURE: 
							{
								LuaObject.Proto p;
								LuaObject.Closure ncl;
								int nup, j;
								p = cl.p.p[LuaOpCodes.GETARG_Bx(i)];
								nup = p.nups;
								ncl = LuaFunc.luaF_newLclosure(L, nup, cl.getEnv());
								ncl.l.p = p;
								for (j = 0; j < nup;) 
								{
									if (LuaOpCodes.GET_OPCODE(pc.get(0)) == LuaOpCodes.OpCode.OP_GETUPVAL)
									{
										ncl.l.upvals[j] = cl.upvals[LuaOpCodes.GETARG_B(pc.get(0))];
									}
									else 
									{
										LuaLimits.lua_assert(LuaOpCodes.GET_OPCODE(pc.get(0)) == LuaOpCodes.OpCode.OP_MOVE);
										ncl.l.upvals[j] = LuaFunc.luaF_findupval(L, LuaObject.TValue.plus(base_, LuaOpCodes.GETARG_B(pc.get(0))));
									}
									
									j++; 
									LuaCode.InstructionPtr[] pc_ref4 = new LuaCode.InstructionPtr[1];
									pc_ref4[0] = pc;
									LuaCode.InstructionPtr.inc(/*ref*/ pc_ref4);
									pc = pc_ref4[0];
								}
								LuaObject.setclvalue(L, ra, ncl);
								//Protect(
								L.savedpc = LuaCode.InstructionPtr.Assign(pc);
								LuaGC.luaC_checkGC(L);
								base_ = L.base_;
								//);
								continue;
							}
						case LuaOpCodes.OpCode.OP_VARARG: 
							{
								int b = LuaOpCodes.GETARG_B(i) - 1;
								int j;
								LuaState.CallInfo ci = L.ci;
								int n = LuaLimits.cast_int(LuaObject.TValue.minus(ci.base_, ci.func)) - cl.p.numparams - 1;
								if (b == Lua.LUA_MULTRET)
								{
									//Protect(
									L.savedpc = LuaCode.InstructionPtr.Assign(pc);
									LuaDo.luaD_checkstack(L, n);
									base_ = L.base_;
									//);
									ra = RA(L, base_, i);  /* previous call may change the stack */
									b = n;
									L.top = LuaObject.TValue.plus(ra, n);
								}
								for (j = 0; j < b; j++) 
								{
									if (j < n) 
									{
										LuaObject.setobjs2s(L, LuaObject.TValue.plus(ra, j), LuaObject.TValue.plus(LuaObject.TValue.minus(ci.base_, n), j)); //FIXME:
									}
									else 
									{
										LuaObject.setnilvalue(LuaObject.TValue.plus(ra, j));
									}
								}
								continue;
							}
					} //end switch
					if (reentry2 == true)
					{
						reentry = true;
						break;
					}
				}//end for
				if (reentry == true)
				{
					continue;
				}
				else
				{
					break;
				}
			}//end while
		}

	}
}
