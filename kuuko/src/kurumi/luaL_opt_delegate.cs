namespace kurumi
{
    public interface luaL_opt_delegate
    {
        /*Double*/
        /*lua_Number*/
        double exec(LuaState.lua_State L, int narg);
    }
}
