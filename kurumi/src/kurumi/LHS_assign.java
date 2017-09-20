/*
 ** $Id: lparser.c,v 2.42.1.3 2007/12/28 15:32:23 roberto Exp $
 ** Lua Parser
 ** See Copyright Notice in lua.h
 */
package kurumi;
//{
	/*
	 ** structure to chain all variables in the left-hand side of an
	 ** assignment
	 */
	public class LHS_assign
	{
		public LHS_assign prev;
		public LuaParser.expdesc v = new LuaParser.expdesc();  /* variable (global, local, upvalue, or indexed) */
	}
//}
