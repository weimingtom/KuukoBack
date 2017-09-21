/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
namespace kurumi
{
	public class NextRef : LuaState.GCObjectRef
	{
		private LuaObject.GCheader header;
		
		public NextRef(LuaObject.GCheader header) 
		{ 
			this.header = header; 
		}
		
		public void set(LuaState.GCObject value) 
		{ 
			this.header.next = value; 
		}
		
		public LuaState.GCObject get() 
		{ 
			return this.header.next; 
		}
	}
}
