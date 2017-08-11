package kurumi;
//{
	public class f_parser_delegate implements Pfunc 
	{
		public f_parser_delegate()
		{
			
		}
		
		public void exec(lua_State L, Object ud)
		{
			LuaDo.f_parser(L, ud);
		}
	}
//}
