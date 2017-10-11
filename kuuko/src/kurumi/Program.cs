namespace kurumi
{
	public class Program
	{
		static int Main(string[] args)
		{
			//args = new string[] {"test/bisect.lua"};
			//args = new string[] {"test/cf.lua"};
			//args = new string[] {"test/echo.lua"};
			//args = new string[] {"test/env.lua"};
			//args = new string[] {"test/factorial.lua"};
			//args = new string[] {"test/fib.lua"};
			//args = new string[] {"test/fibfor.lua"};
			//args = new string[] {"test/globals.lua"}; // not tested
			//args = new string[] {"test/hello.lua"};
			//args = new string[] {"test/life.lua"};
			//args = new string[] {"test/luac.lua"}; // not tested
			//args = new string[] {"test/printf.lua"}; // error
			//args = new string[] {"test/readonly.lua"};
			//args = new string[] {"-e", "N=100", "test/sieve.lua"};
			//args = new string[] {"test/sort.lua"};
			//args = new string[] {"test/table.lua"}; // not tested
			//args = new string[] {"test/trace-calls.lua"}; // not tested
			//args = new string[] {"test/trace-globals.lua"};
			//args = new string[] {"test/xd.lua"}; // not tested
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
