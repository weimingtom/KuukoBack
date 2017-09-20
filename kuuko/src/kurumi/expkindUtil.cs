using System;
using System.Collections.Generic;
using System.Text;

namespace kurumi
{
    public class expkindUtil
    {
        public static int expkindToInt(LuaParser.expkind exp)
        {
            switch (exp)
            {
                case LuaParser.expkind.VVOID:
                    return 0;
		        case LuaParser.expkind.VNIL:
                    return 1;
		        case LuaParser.expkind.VTRUE:
                    return 2;
		        case LuaParser.expkind.VFALSE:
                    return 3;
		        case LuaParser.expkind.VK:
                    return 4;		
		        case LuaParser.expkind.VKNUM:
                    return 5;	
		        case LuaParser.expkind.VLOCAL:
                    return 6;	
		        case LuaParser.expkind.VUPVAL:
                    return 7;       
		        case LuaParser.expkind.VGLOBAL:
                    return 8;	
		        case LuaParser.expkind.VINDEXED:
                    return 9;	
		        case LuaParser.expkind.VJMP:
                    return 10;		
		        case LuaParser.expkind.VRELOCABLE:
                    return 11;	
		        case LuaParser.expkind.VNONRELOC:
                    return 12;	
		        case LuaParser.expkind.VCALL:
                    return 13;
                case LuaParser.expkind.VVARARG:
                    return 14;	
            }
            throw new Exception("expkindToInt error");
        }
    }
}
