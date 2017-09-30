using System;
using System.Collections.Generic;
using System.Text;

namespace kurumi
{
    public class TMSUtil
    {
        public static int convertTMStoInt(LuaTM.TMS tms)
        {
            switch (tms)
            {
                case LuaTM.TMS.TM_INDEX:
                    return 0;
                case LuaTM.TMS.TM_NEWINDEX:
                    return 1;
                case LuaTM.TMS.TM_GC:
                    return 2;
                case LuaTM.TMS.TM_MODE:
                    return 3;
                case LuaTM.TMS.TM_EQ:
                    return 4;
                case LuaTM.TMS.TM_ADD:
                    return 5;
                case LuaTM.TMS.TM_SUB:
                    return 6;
                case LuaTM.TMS.TM_MUL:
                    return 7;
                case LuaTM.TMS.TM_DIV:
                    return 8;
                case LuaTM.TMS.TM_MOD:
                    return 9;
                case LuaTM.TMS.TM_POW:
                    return 10;
                case LuaTM.TMS.TM_UNM:
                    return 11;
                case LuaTM.TMS.TM_LEN:
                    return 12;
                case LuaTM.TMS.TM_LT:
                    return 13;
                case LuaTM.TMS.TM_LE:
                    return 14;
                case LuaTM.TMS.TM_CONCAT:
                    return 15;
                case LuaTM.TMS.TM_CALL:
                    return 16;
                case LuaTM.TMS.TM_N:
                    return 17;
            }
            throw new Exception("convertTMStoInt error");
        }
    }
}
