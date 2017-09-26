/*
 ** $Id: lmathlib.c,v 1.67.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Standard mathematical library
 ** See Copyright Notice in lua.h
 */
using System;

namespace kurumi
{
	//using lua_Number = System.Double;

	public class LuaMathLib
	{
		public const double PI = 3.14159265358979323846;
		public const double RADIANS_PER_DEGREE = PI / 180.0;

		private static int math_abs(LuaState.lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Abs(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_sin(LuaState.lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Sin(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_sinh(LuaState.lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Sinh(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_cos(LuaState.lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Cos(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_cosh(LuaState.lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Cosh(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_tan(LuaState.lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Tan(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_tanh(LuaState.lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Tanh(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_asin(LuaState.lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Asin(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_acos(LuaState.lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Acos(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_atan(LuaState.lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Atan(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_atan2(LuaState.lua_State L) 
		{
			LuaAPI.lua_pushnumber(L, Math.Atan2(LuaAuxLib.luaL_checknumber(L, 1), LuaAuxLib.luaL_checknumber(L, 2)));
			return 1;
		}

		private static int math_ceil(LuaState.lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Ceiling(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_floor(LuaState.lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Floor(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_fmod(LuaState.lua_State L)
		{
			LuaAPI.lua_pushnumber(L, LuaConf.fmod(LuaAuxLib.luaL_checknumber(L, 1), LuaAuxLib.luaL_checknumber(L, 2)));
			return 1;
		}

		private static int math_modf(LuaState.lua_State L) 
		{
			double[] ip = new double[1];
			double fp = LuaConf.modf(LuaAuxLib.luaL_checknumber(L, 1), /*out*/ ip);
			LuaAPI.lua_pushnumber(L, ip[0]);
			LuaAPI.lua_pushnumber(L, fp);
			return 2;
		}

		private static int math_sqrt(LuaState.lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Sqrt(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_pow(LuaState.lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Pow(LuaAuxLib.luaL_checknumber(L, 1), LuaAuxLib.luaL_checknumber(L, 2)));
			return 1;
		}

		private static int math_log(LuaState.lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Log(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_log10(LuaState.lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Log10(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_exp(LuaState.lua_State L)
		{
			LuaAPI.lua_pushnumber(L, Math.Exp(LuaAuxLib.luaL_checknumber(L, 1)));
			return 1;
		}

		private static int math_deg(LuaState.lua_State L)
		{
			LuaAPI.lua_pushnumber(L, LuaAuxLib.luaL_checknumber(L, 1) / RADIANS_PER_DEGREE);
			return 1;
		}

		private static int math_rad(LuaState.lua_State L)
		{
			LuaAPI.lua_pushnumber(L, LuaAuxLib.luaL_checknumber(L, 1) * RADIANS_PER_DEGREE);
			return 1;
		}

		private static int math_frexp(LuaState.lua_State L)
		{
			int[] e = new int[1];
			LuaAPI.lua_pushnumber(L, LuaConf.frexp(LuaAuxLib.luaL_checknumber(L, 1), /*out*/ e));
			LuaAPI.lua_pushinteger(L, e[0]);
			return 2;
		}

		private static int math_ldexp(LuaState.lua_State L)
		{
			LuaAPI.lua_pushnumber(L, LuaConf.ldexp(LuaAuxLib.luaL_checknumber(L, 1), LuaAuxLib.luaL_checkint(L, 2)));
			return 1;
		}

		private static int math_min(LuaState.lua_State L)
		{
			int n = LuaAPI.lua_gettop(L);  /* number of arguments */
			Double/*lua_Number*/ dmin = LuaAuxLib.luaL_checknumber(L, 1);
			int i;
			for (i = 2; i <= n; i++) 
			{
				Double/*lua_Number*/ d = LuaAuxLib.luaL_checknumber(L, i);
				if (d < dmin)
				{
					dmin = d;
				}
			}
			LuaAPI.lua_pushnumber(L, dmin);
			return 1;
		}


		private static int math_max(LuaState.lua_State L) 
		{
			int n = LuaAPI.lua_gettop(L);  /* number of arguments */
			Double/*lua_Number*/ dmax = LuaAuxLib.luaL_checknumber(L, 1);
			int i;
			for (i = 2; i <= n; i++) 
			{
				Double/*lua_Number*/ d = LuaAuxLib.luaL_checknumber(L, i);
				if (d > dmax)
				{
					dmax = d;
				}
			}
			LuaAPI.lua_pushnumber(L, dmax);
			return 1;
		}

		private static Random rng = new Random();

		private static int math_random(LuaState.lua_State L) 
		{
			/* the `%' avoids the (rare) case of r==1, and is needed also because on
			 some systems (SunOS!) `rand()' may return a value larger than RAND_MAX */
			//lua_Number r = (lua_Number)(rng.Next()%RAND_MAX) / (lua_Number)RAND_MAX;
			Double/*lua_Number*/ r = (Double/*lua_Number*/)rng.NextDouble();
			switch (LuaAPI.lua_gettop(L))
			{  
				/* check number of arguments */
				case 0: 
					{  
						/* no arguments */
						LuaAPI.lua_pushnumber(L, r);  /* Number between 0 and 1 */
						break;
					}
				case 1: 
					{  
						/* only upper limit */
						int u = LuaAuxLib.luaL_checkint(L, 1);
						LuaAuxLib.luaL_argcheck(L, 1 <= u, 1, "interval is empty");
						LuaAPI.lua_pushnumber(L, Math.Floor(r * u) + 1);  /* int between 1 and `u' */
						break;
					}
				case 2: 
					{  
						/* lower and upper limits */
						int l = LuaAuxLib.luaL_checkint(L, 1);
						int u = LuaAuxLib.luaL_checkint(L, 2);
						LuaAuxLib.luaL_argcheck(L, l <= u, 2, "interval is empty");
						LuaAPI.lua_pushnumber(L, Math.Floor(r * (u - l + 1)) + l);  /* int between `l' and `u' */
						break;
					}
				default: 
					{
						return LuaAuxLib.luaL_error(L, LuaConf.CharPtr.toCharPtr("wrong number of arguments"));
					}
			}
			return 1;
		}


		private static int math_randomseed(LuaState.lua_State L) 
		{
			//srand(luaL_checkint(L, 1));
			rng = new Random(LuaAuxLib.luaL_checkint(L, 1));
			return 0;
		}

		private readonly static LuaAuxLib.luaL_Reg[] mathlib = {
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("abs"), new LuaMathLib_delegate("math_abs")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("acos"), new LuaMathLib_delegate("math_acos")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("asin"), new LuaMathLib_delegate("math_asin")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("atan2"), new LuaMathLib_delegate("math_atan2")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("atan"), new LuaMathLib_delegate("math_atan")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("ceil"), new LuaMathLib_delegate("math_ceil")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("cosh"), new LuaMathLib_delegate("math_cosh")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("cos"), new LuaMathLib_delegate("math_cos")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("deg"), new LuaMathLib_delegate("math_deg")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("exp"), new LuaMathLib_delegate("math_exp")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("floor"), new LuaMathLib_delegate("math_floor")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("fmod"), new LuaMathLib_delegate("math_fmod")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("frexp"), new LuaMathLib_delegate("math_frexp")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("ldexp"), new LuaMathLib_delegate("math_ldexp")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("log10"), new LuaMathLib_delegate("math_log10")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("log"), new LuaMathLib_delegate("math_log")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("max"), new LuaMathLib_delegate("math_max")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("min"), new LuaMathLib_delegate("math_min")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("modf"), new LuaMathLib_delegate("math_modf")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("pow"), new LuaMathLib_delegate("math_pow")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("rad"), new LuaMathLib_delegate("math_rad")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("random"), new LuaMathLib_delegate("math_random")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("randomseed"), new LuaMathLib_delegate("math_randomseed")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("sinh"), new LuaMathLib_delegate("math_sinh")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("sin"), new LuaMathLib_delegate("math_sin")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("sqrt"), new LuaMathLib_delegate("math_sqrt")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("tanh"), new LuaMathLib_delegate("math_tanh")),
			new LuaAuxLib.luaL_Reg(LuaConf.CharPtr.toCharPtr("tan"), new LuaMathLib_delegate("math_tan")),
			new LuaAuxLib.luaL_Reg(null, null)
		};

		public class LuaMathLib_delegate : Lua.lua_CFunction
		{
			private string name;
			
			public LuaMathLib_delegate(string name)
			{
				this.name = name;
			}
			
			public int exec(LuaState.lua_State L)
			{
				if ("math_abs".Equals(name))
				{
					return math_abs(L);
				} 
				else if ("math_acos".Equals(name)) 
				{
					return math_acos(L);
				} 
				else if ("math_asin".Equals(name)) 
				{
					return math_asin(L);
				} 
				else if ("math_atan2".Equals(name)) 
				{
				    return math_atan2(L);
				}
				else if ("math_atan".Equals(name))
				{
				    return math_atan(L);
				}
				else if ("math_ceil".Equals(name))
				{
					return math_ceil(L);
				}
				else if ("math_cosh".Equals(name))
				{
					return math_cosh(L);
				}
				else if ("math_cos".Equals(name))
				{
					return math_cos(L);
				}
				else if ("math_deg".Equals(name))
				{
					return math_deg(L);
				}
				else if ("math_exp".Equals(name))
				{
					return math_exp(L);
				}
				else if ("math_floor".Equals(name))
				{
					return math_floor(L);
				}
				else if ("math_fmod".Equals(name))
				{
					return math_fmod(L);
				}
				else if ("math_frexp".Equals(name))
				{
					return math_frexp(L);
				}
				else if ("math_ldexp".Equals(name))
				{
					return math_ldexp(L);
				}
				else if ("math_log10".Equals(name))
				{
					return math_log10(L);
				}
				else if ("math_log".Equals(name))
				{
					return math_log(L);
				}
				else if ("math_max".Equals(name))
				{
					return math_max(L);
				}
				else if ("math_min".Equals(name))
				{
					return math_min(L);
				}
				else if ("math_modf".Equals(name))
				{
					return math_modf(L);
				}
				else if ("math_pow".Equals(name))
				{
					return math_pow(L);
				}
				else if ("math_rad".Equals(name))
				{
					return math_rad(L);
				}
				else if ("math_random".Equals(name))
				{
					return math_random(L);
				}
				else if ("math_randomseed".Equals(name))
				{
					return math_randomseed(L);
				}
				else if ("math_sinh".Equals(name))
				{
					return math_sinh(L);
				}
				else if ("math_sin".Equals(name))
				{
					return math_sin(L);
				}
				else if ("math_sqrt".Equals(name))
				{
					return math_sqrt(L);
				}
				else if ("math_tanh".Equals(name))
				{
					return math_tanh(L);
				}
				else if ("math_tan".Equals(name))
				{
					return math_tan(L);
				}
				else
				{
					return 0;
				}
			}
		}
		
		
		/*
		 ** Open math library
		 */
		public static int luaopen_math (LuaState.lua_State L)
		{
			LuaAuxLib.luaL_register(L, LuaConf.CharPtr.toCharPtr(LuaLib.LUA_MATHLIBNAME), mathlib);
			LuaAPI.lua_pushnumber(L, PI);
			LuaAPI.lua_setfield(L, -2, LuaConf.CharPtr.toCharPtr("pi"));
			LuaAPI.lua_pushnumber(L, LuaConf.HUGE_VAL);
			LuaAPI.lua_setfield(L, -2, LuaConf.CharPtr.toCharPtr("huge"));
			//#if LUA_COMPAT_MOD
			LuaAPI.lua_getfield(L, -1, LuaConf.CharPtr.toCharPtr("fmod"));
			LuaAPI.lua_setfield(L, -2, LuaConf.CharPtr.toCharPtr("mod"));
			//#endif
			return 1;
		}
	}
}
