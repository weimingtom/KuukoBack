﻿/*
 ** $Id: llex.c,v 2.20.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Lexical Analyzer
 ** See Copyright Notice in lua.h
 */
namespace kurumi
{
	public class Token
	{
		public int token;
		public LuaLex.SemInfo seminfo = new LuaLex.SemInfo();
		
		public Token()
		{
			
		}
		
		public Token(Token copy)
		{
			this.token = copy.token;
			this.seminfo = new LuaLex.SemInfo(copy.seminfo);
		}
	}
}
