namespace kurumi
{
	public class Program
	{
		static int Main(string[] args)
		{
			string LUA_ENABLE_LUAC = System.Environment.GetEnvironmentVariable("LUA_ENABLE_LUAC");
			
			//args = new string[] {"test/bisect.lua"};
			//args = new string[] {"test/cf.lua"};
			//args = new string[] {"test/echo.lua"};
			//args = new string[] {"test/env.lua"};
			//args = new string[] {"test/factorial.lua"};
			//args = new string[] {"test/fib.lua"};
			//args = new string[] {"test/fibfor.lua"};
			//args = new string[] {"test/globals.lua"}; //remote test
			//args = new string[] {"test/hello.lua"};
			//args = new string[] {"test/life.lua"};
			//args = new string[] {"test/luac.lua", "test/luac/sample.lua"};  //remote test
			//args = new string[] {"test/printf.lua"};
			//args = new string[] {"test/readonly.lua"};
			//args = new string[] {"-e", "N=100", "test/sieve.lua"};
			//args = new string[] {"test/sieve.lua"};
			//args = new string[] {"test/sort.lua"};
			//args = new string[] {"test/table.lua"}; //remote test
			//args = new string[] {"-ltest/trace-calls", "test/bisect.lua"};
			//args = new string[] {"test/trace-globals.lua"};
			//args = new string[] {"test/xd.lua"}; //remote test
			if (LUA_ENABLE_LUAC != null && LUA_ENABLE_LUAC.Length > 0)
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
