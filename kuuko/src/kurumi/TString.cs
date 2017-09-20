﻿/*
 ** $Id: lobject.c,v 2.22.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Some generic functions over Lua objects
 ** See Copyright Notice in lua.h
 */
namespace kurumi
{
	public class TString : TString_tsv
	{
		public LuaConf.CharPtr str;
		
		//public L_Umaxalign dummy;  /* ensures maximum alignment for strings */

        public TString_tsv getTsv()
        {
            return this;
        }

		public TString()
		{
			
		}
		
		public TString(LuaConf.CharPtr str) 
		{ 
			this.str = str; 
		}
		
		public override string ToString() 
		{ 
			return str.ToString(); 
		} // for debugging
	}
}
