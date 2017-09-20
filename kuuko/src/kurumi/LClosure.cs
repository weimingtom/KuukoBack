﻿/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace kurumi
{
	public class LClosure : LuaObject.ClosureType
	{	
		public Proto p;
		public UpVal[] upvals;
		
		public LClosure(LuaObject.ClosureHeader header) : base(header) 
		{
				
		}
	}
}
