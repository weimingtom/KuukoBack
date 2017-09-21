/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
package kurumi;
//{
	public class OpenValRef implements LuaState.GCObjectRef 
	{
		private lua_State L;
		
		public OpenValRef(lua_State L) 
		{ 
			this.L = L; 
		}
		
		public void set(LuaState.GCObject value) 
		{ 
			this.L.openupval = value; 
		}
		
		public LuaState.GCObject get() 
		{ 
			return this.L.openupval; 
		}
	}
//}
