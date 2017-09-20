/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace kurumi
{
	public class Closure : ClosureHeader
	{	
		public LuaObject.CClosure c;
		public LClosure l;
		
		public Closure()
		{
			c = new LuaObject.CClosure(this);
			l = new LClosure(this);
		}
	}
}
