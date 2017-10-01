package kurumi;

//
// ** $Id: lmathlib.c,v 1.67.1.1 2007/12/27 13:02:25 roberto Exp $
// ** Standard mathematical library
// ** See Copyright Notice in lua.h
// 

//using lua_Number = System.Double;

public class LuaMathLib {
	public static final double PI = 3.14159265358979323846;
	public static final double RADIANS_PER_DEGREE = PI / 180.0;

	private static int math_abs(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.abs(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_sin(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.sin(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_sinh(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.sinh(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_cos(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.cos(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_cosh(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.cosh(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_tan(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.tan(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_tanh(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.tanh(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_asin(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.asin(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_acos(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.acos(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_atan(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.atan(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_atan2(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.atan2(LuaAuxLib.luaL_checknumber(L, 1), LuaAuxLib.luaL_checknumber(L, 2)));
		return 1;
	}

	private static int math_ceil(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.ceil(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_floor(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.floor(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_fmod(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, CLib.fmod(LuaAuxLib.luaL_checknumber(L, 1), LuaAuxLib.luaL_checknumber(L, 2)));
		return 1;
	}

	private static int math_modf(LuaState.lua_State L) {
		double[] ip = new double[1];
		double fp = CLib.modf(LuaAuxLib.luaL_checknumber(L, 1), ip); //out
		LuaAPI.lua_pushnumber(L, ip[0]);
		LuaAPI.lua_pushnumber(L, fp);
		return 2;
	}

	private static int math_sqrt(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.sqrt(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_pow(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.pow(LuaAuxLib.luaL_checknumber(L, 1), LuaAuxLib.luaL_checknumber(L, 2)));
		return 1;
	}

	private static int math_log(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.log(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_log10(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.log10(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_exp(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, Math.exp(LuaAuxLib.luaL_checknumber(L, 1)));
		return 1;
	}

	private static int math_deg(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, LuaAuxLib.luaL_checknumber(L, 1) / RADIANS_PER_DEGREE);
		return 1;
	}

	private static int math_rad(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, LuaAuxLib.luaL_checknumber(L, 1) * RADIANS_PER_DEGREE);
		return 1;
	}

	private static int math_frexp(LuaState.lua_State L) {
		int[] e = new int[1];
		LuaAPI.lua_pushnumber(L, CLib.frexp(LuaAuxLib.luaL_checknumber(L, 1), e)); //out
		LuaAPI.lua_pushinteger(L, e[0]);
		return 2;
	}

	private static int math_ldexp(LuaState.lua_State L) {
		LuaAPI.lua_pushnumber(L, CLib.ldexp(LuaAuxLib.luaL_checknumber(L, 1), LuaAuxLib.luaL_checkint(L, 2)));
		return 1;
	}

	private static int math_min(LuaState.lua_State L) {
		int n = LuaAPI.lua_gettop(L); // number of arguments 
		double dmin = LuaAuxLib.luaL_checknumber(L, 1); //lua_Number
		int i;
		for (i = 2; i <= n; i++) {
			double d = LuaAuxLib.luaL_checknumber(L, i); //lua_Number
			if (d < dmin) {
				dmin = d;
			}
		}
		LuaAPI.lua_pushnumber(L, dmin);
		return 1;
	}


	private static int math_max(LuaState.lua_State L) {
		int n = LuaAPI.lua_gettop(L); // number of arguments 
		double dmax = LuaAuxLib.luaL_checknumber(L, 1); //lua_Number
		int i;
		for (i = 2; i <= n; i++) {
			double d = LuaAuxLib.luaL_checknumber(L, i); //lua_Number
			if (d > dmax) {
				dmax = d;
			}
		}
		LuaAPI.lua_pushnumber(L, dmax);
		return 1;
	}

	private static java.util.Random rng = new java.util.Random();

	private static int math_random(LuaState.lua_State L) {
//             the `%' avoids the (rare) case of r==1, and is needed also because on
//			 some systems (SunOS!) `rand()' may return a value larger than RAND_MAX 
		//lua_Number r = (lua_Number)(rng.Next()%RAND_MAX) / (lua_Number)RAND_MAX;
		double r = (double)rng.nextDouble(); //lua_Number - lua_Number
		switch (LuaAPI.lua_gettop(L)) {
			// check number of arguments 
			case 0: {
					// no arguments 
					LuaAPI.lua_pushnumber(L, r); // Number between 0 and 1 
					break;
				}
			case 1: {
					// only upper limit 
					int u = LuaAuxLib.luaL_checkint(L, 1);
					LuaAuxLib.luaL_argcheck(L, 1 <= u, 1, "interval is empty");
					LuaAPI.lua_pushnumber(L, Math.floor(r * u) + 1); // int between 1 and `u' 
					break;
				}
			case 2: {
					// lower and upper limits 
					int l = LuaAuxLib.luaL_checkint(L, 1);
					int u = LuaAuxLib.luaL_checkint(L, 2);
					LuaAuxLib.luaL_argcheck(L, l <= u, 2, "interval is empty");
					LuaAPI.lua_pushnumber(L, Math.floor(r * (u - l + 1)) + l); // int between `l' and `u' 
					break;
				}
			default: {
					return LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("wrong number of arguments"));
				}
		}
		return 1;
	}


	private static int math_randomseed(LuaState.lua_State L) {
		//srand(luaL_checkint(L, 1));
		rng = new java.util.Random(LuaAuxLib.luaL_checkint(L, 1));
		return 0;
	}

	private final static LuaAuxLib.luaL_Reg[] mathlib = { 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("abs"), new LuaMathLib_delegate("math_abs")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("acos"), new LuaMathLib_delegate("math_acos")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("asin"), new LuaMathLib_delegate("math_asin")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("atan2"), new LuaMathLib_delegate("math_atan2")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("atan"), new LuaMathLib_delegate("math_atan")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("ceil"), new LuaMathLib_delegate("math_ceil")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("cosh"), new LuaMathLib_delegate("math_cosh")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("cos"), new LuaMathLib_delegate("math_cos")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("deg"), new LuaMathLib_delegate("math_deg")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("exp"), new LuaMathLib_delegate("math_exp")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("floor"), new LuaMathLib_delegate("math_floor")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("fmod"), new LuaMathLib_delegate("math_fmod")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("frexp"), new LuaMathLib_delegate("math_frexp")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("ldexp"), new LuaMathLib_delegate("math_ldexp")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("log10"), new LuaMathLib_delegate("math_log10")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("log"), new LuaMathLib_delegate("math_log")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("max"), new LuaMathLib_delegate("math_max")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("min"), new LuaMathLib_delegate("math_min")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("modf"), new LuaMathLib_delegate("math_modf")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("pow"), new LuaMathLib_delegate("math_pow")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("rad"), new LuaMathLib_delegate("math_rad")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("random"), new LuaMathLib_delegate("math_random")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("randomseed"), new LuaMathLib_delegate("math_randomseed")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("sinh"), new LuaMathLib_delegate("math_sinh")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("sin"), new LuaMathLib_delegate("math_sin")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("sqrt"), new LuaMathLib_delegate("math_sqrt")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("tanh"), new LuaMathLib_delegate("math_tanh")), 
		new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("tan"), new LuaMathLib_delegate("math_tan")), 
		new LuaAuxLib.luaL_Reg(null, null) 
	};

	public static class LuaMathLib_delegate implements Lua.lua_CFunction {
		private String name;

		public LuaMathLib_delegate(String name) {
			this.name = name;
		}

		public final int exec(LuaState.lua_State L) {
			if ((new String("math_abs")).equals(name)) {
				return math_abs(L);
			}
			else if ((new String("math_acos")).equals(name)) {
				return math_acos(L);
			}
			else if ((new String("math_asin")).equals(name)) {
				return math_asin(L);
			}
			else if ((new String("math_atan2")).equals(name)) {
				return math_atan2(L);
			}
			else if ((new String("math_atan")).equals(name)) {
				return math_atan(L);
			}
			else if ((new String("math_ceil")).equals(name)) {
				return math_ceil(L);
			}
			else if ((new String("math_cosh")).equals(name)) {
				return math_cosh(L);
			}
			else if ((new String("math_cos")).equals(name)) {
				return math_cos(L);
			}
			else if ((new String("math_deg")).equals(name)) {
				return math_deg(L);
			}
			else if ((new String("math_exp")).equals(name)) {
				return math_exp(L);
			}
			else if ((new String("math_floor")).equals(name)) {
				return math_floor(L);
			}
			else if ((new String("math_fmod")).equals(name)) {
				return math_fmod(L);
			}
			else if ((new String("math_frexp")).equals(name)) {
				return math_frexp(L);
			}
			else if ((new String("math_ldexp")).equals(name)) {
				return math_ldexp(L);
			}
			else if ((new String("math_log10")).equals(name)) {
				return math_log10(L);
			}
			else if ((new String("math_log")).equals(name)) {
				return math_log(L);
			}
			else if ((new String("math_max")).equals(name)) {
				return math_max(L);
			}
			else if ((new String("math_min")).equals(name)) {
				return math_min(L);
			}
			else if ((new String("math_modf")).equals(name)) {
				return math_modf(L);
			}
			else if ((new String("math_pow")).equals(name)) {
				return math_pow(L);
			}
			else if ((new String("math_rad")).equals(name)) {
				return math_rad(L);
			}
			else if ((new String("math_random")).equals(name)) {
				return math_random(L);
			}
			else if ((new String("math_randomseed")).equals(name)) {
				return math_randomseed(L);
			}
			else if ((new String("math_sinh")).equals(name)) {
				return math_sinh(L);
			}
			else if ((new String("math_sin")).equals(name)) {
				return math_sin(L);
			}
			else if ((new String("math_sqrt")).equals(name)) {
				return math_sqrt(L);
			}
			else if ((new String("math_tanh")).equals(name)) {
				return math_tanh(L);
			}
			else if ((new String("math_tan")).equals(name)) {
				return math_tan(L);
			}
			else {
				return 0;
			}
		}
	}


//        
//		 ** Open math library
//		 
	public static int luaopen_math(LuaState.lua_State L) {
		LuaAuxLib.luaL_register(L, CLib.CharPtr.toCharPtr(LuaLib.LUA_MATHLIBNAME), mathlib);
		LuaAPI.lua_pushnumber(L, PI);
		LuaAPI.lua_setfield(L, -2, CLib.CharPtr.toCharPtr("pi"));
		LuaAPI.lua_pushnumber(L, CLib.HUGE_VAL);
		LuaAPI.lua_setfield(L, -2, CLib.CharPtr.toCharPtr("huge"));
		///#if LUA_COMPAT_MOD
		LuaAPI.lua_getfield(L, -1, CLib.CharPtr.toCharPtr("fmod"));
		LuaAPI.lua_setfield(L, -2, CLib.CharPtr.toCharPtr("mod"));
		///#endif
		return 1;
	}
}