/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
package kurumi;
//{
	/*
	 ** Main thread combines a thread state and the global state
	 */
	public class LG extends lua_State 
	{
		public LuaState.global_State g = new LuaState.global_State();
		
		public lua_State getL() 
		{
		    return this; 
		}
	}
//}
