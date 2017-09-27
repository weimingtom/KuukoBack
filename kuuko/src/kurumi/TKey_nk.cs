﻿/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace kurumi
{
	/*
	 ** Tables
	 */
	public class TKey_nk : TValue
	{
		public LuaObject.Node next;  /* for chaining */
		
		public TKey_nk() 
		{
			
		}
		
		public TKey_nk(Value value, int tt, LuaObject.Node next)
		: base(new Value(value), tt)
		{
			this.next = next;
		}
	}
}
