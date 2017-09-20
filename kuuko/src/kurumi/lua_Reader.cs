namespace kurumi
{
    public interface lua_Reader
    {
        /*sz*/
        /*out*/
        /*uint*/
        LuaConf.CharPtr exec(lua_State L, object ud, int[] sz);
    }
}
