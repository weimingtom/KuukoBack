package kurumi;
//{
	public class LuaStrLib_delegate implements Lua.lua_CFunction 
	{
		private String name;
		
		public LuaStrLib_delegate(String name) 
		{
			this.name = name;
		}
		
		public int exec(lua_State L)
		{
			if ("str_byte".equals(name)) 
			{
				return LuaStrLib.str_byte(L);
			} 
			else if ("str_char".equals(name)) 
			{
				return LuaStrLib.str_char(L);
			} 
			else if ("str_dump".equals(name)) 
			{
				return LuaStrLib.str_dump(L);
			} 
			else if ("str_find".equals(name)) 
			{
			    return LuaStrLib.str_find(L);
			}
			else if ("str_format".equals(name)) 
			{
			    return LuaStrLib.str_format(L);
			}
			else if ("gfind_nodef".equals(name)) 
			{
				return LuaStrLib.gfind_nodef(L);
			}
			else if ("gmatch".equals(name)) 
			{
				return LuaStrLib.gmatch(L);
			}
			else if ("str_gsub".equals(name)) 
			{
				return LuaStrLib.str_gsub(L);
			}
			else if ("str_len".equals(name)) 
			{
				return LuaStrLib.str_len(L);
			}
			else if ("str_lower".equals(name)) 
			{
				return LuaStrLib.str_lower(L);
			}
			else if ("str_match".equals(name)) 
			{
				return LuaStrLib.str_match(L);
			}
			else if ("str_rep".equals(name)) 
			{
				return LuaStrLib.str_rep(L);
			}
			else if ("str_reverse".equals(name)) 
			{
				return LuaStrLib.str_reverse(L);
			}
			else if ("str_sub".equals(name)) 
			{
				return LuaStrLib.str_sub(L);
			}
			else if ("str_upper".equals(name)) 
			{
				return LuaStrLib.str_upper(L);
			}
			else if ("gmatch_aux".equals(name)) 
			{
				return LuaStrLib.gmatch_aux(L);
			}
			else
			{
				return 0;
			}
		}
	}
//}
