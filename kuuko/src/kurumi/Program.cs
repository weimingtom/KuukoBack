namespace kurumi
{
	public class Program
	{
		static int Main(string[] args)
		{
			//args = new string[] {"test/fib.lua"};
			if (false)
			{
				LuacProgram.MainLuac(args);
			}
			else
			{
				LuaProgram.MainLua(args);
			}
			return 0;
		}
	}
}
