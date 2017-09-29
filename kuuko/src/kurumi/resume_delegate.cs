namespace kurumi
{
	public class resume_delegate : LuaDo.Pfunc
	{
		public void exec(LuaState.lua_State L, object ud)
		{
			LuaDo.resume(L, ud);
		}
	}
}
