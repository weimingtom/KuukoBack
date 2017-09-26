namespace kurumi
{
	public interface Pfunc
	{
		void exec(LuaState.lua_State L, object ud);
	} 
}
