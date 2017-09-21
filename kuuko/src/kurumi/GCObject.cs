﻿/*
 ** $Id: lstate.c,v 2.36.1.2 2008/01/03 15:20:39 roberto Exp $
 ** Global State
 ** See Copyright Notice in lua.h
 */
namespace kurumi
{
	/*
	 ** Union of all collectable objects (not a union anymore in the C# port)
	 */
	public class GCObject : LuaObject.GCheader, LuaObject.ArrayElement
	{
		// todo: remove this?
		//private GCObject[] values = null;
		//private int index = -1;
		
		public void set_index(int index)
		{
			//this.index = index;
		}
		
		public void set_array(object array)
		{
			//this.values = (GCObject[])array;
			//ClassType.Assert(this.values != null);
		}

        public LuaObject.GCheader getGch()
        {
            return (LuaObject.GCheader)this;
        }

        public TString getTs()
        {
            return (TString)this;
        }

        public Udata getU()
        {
            return (Udata)this;
        }

        public LuaObject.Closure getCl()
        {
            return (LuaObject.Closure)this;
        }

        public Table getH()
        {
            return (Table)this;
        }

        public Proto getP()
        {
            return (Proto)this;
        }

        public UpVal getUv()
        {
            return (UpVal)this;
        }

        public lua_State getTh()
        {
            return (lua_State)this;
        }
	}
}
