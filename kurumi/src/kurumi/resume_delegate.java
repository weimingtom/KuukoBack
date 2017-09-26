package kurumi;
//{
	public class resume_delegate implements Pfunc 
	{
		public void exec(LuaState.lua_State L, Object ud) 
		{
			LuaDo.resume(L, ud);
		}
	}
//}
