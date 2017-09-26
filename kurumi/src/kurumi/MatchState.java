/*
 ** $Id: lstrlib.c,v 1.132.1.4 2008/07/11 17:27:21 roberto Exp $
 ** Standard library for string operations and pattern-matching
 ** See Copyright Notice in lua.h
 */
package kurumi;
//{
	public class MatchState
	{
		public static class capture_ 
		{
			public LuaConf.CharPtr init;
            public int/*Int32*//*ptrdiff_t*/ len;
		}
		
		public LuaConf.CharPtr src_init;  /* init of source string */
		public LuaConf.CharPtr src_end;  /* end (`\0') of source string */
		public LuaState.lua_State L;
		public int level;  /* total number of captures (finished or unfinished) */

		public capture_[] capture = new capture_[LuaConf.LUA_MAXCAPTURES];
		
		public MatchState()
		{
			for (int i = 0; i < LuaConf.LUA_MAXCAPTURES; i++)
			{
				capture[i] = new capture_();
			}
		}
	}
//}
