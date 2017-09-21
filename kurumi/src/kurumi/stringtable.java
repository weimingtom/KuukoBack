/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
package kurumi;
//{
	public class stringtable
	{
		public LuaState.GCObject[] hash;
		public long/*UInt32*//*lu_mem*/ nuse;  /* number of elements */
		public int size;
	}
//}
