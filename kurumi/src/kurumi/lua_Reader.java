package kurumi;
//{
    public interface lua_Reader
    {
        /*sz*/
        /*out*/
        /*uint*/
    	LuaConf.CharPtr exec(lua_State L, Object ud, int[] sz);
    }
//}
