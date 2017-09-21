/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
package kurumi;
//{
	public class RootGCRef implements LuaState.GCObjectRef 
	{
		private LuaState.global_State g;
		
		public RootGCRef(LuaState.global_State g) 
		{ 
			this.g = g; 
		}
		
		public void set(LuaState.GCObject value) 
		{ 
			this.g.rootgc = value; 
		}
		
		public LuaState.GCObject get() 
		{ 
			return this.g.rootgc; 
		}
	}
//}
