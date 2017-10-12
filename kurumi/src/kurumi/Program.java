package kurumi;
//{
	public class Program 
	{
		public static void main(String[] args) 
		{
			String LUA_ENABLE_LUAC = System.getenv("LUA_ENABLE_LUAC");
			
			//args = new String[] {"test/bisect.lua"};
			//args = new String[] {"test/cf.lua"};
			//args = new String[] {"test/echo.lua"};
			//args = new String[] {"test/env.lua"};
			//args = new String[] {"test/factorial.lua"};
			//args = new String[] {"test/fib.lua"};
			//args = new String[] {"test/fibfor.lua"};
			//args = new String[] {"test/globals.lua"};
			//args = new String[] {"test/hello.lua"};
			//args = new String[] {"test/life.lua"};
			//args = new String[] {"test/luac.lua", "test/luac/sample.lua"}; // throw error
			//args = new String[] {"test/printf.lua"};
			//args = new String[] {"test/readonly.lua"};
			//args = new String[] {"-e", "N=100", "test/sieve.lua"}; // if N=1000 and LUAI_MAXCCALLS=200, throw java.lang.StackOverflowError, use -Xss4096k
			//args = new String[] {"test/sieve.lua"};
			//args = new String[] {"test/sort.lua"};
			//args = new String[] {"test/table.lua"};
			//args = new String[] {"-ltest/trace-calls", "test/bisect.lua"};
			//args = new String[] {"test/trace-globals.lua"};
			//args = new String[] {"test/xd.lua"};
			if (LUA_ENABLE_LUAC != null && LUA_ENABLE_LUAC.length() > 0)
			{
				LuacProgram.MainLuac(args);
			}
			else
			{
				LuaProgram.MainLua(args);
			}
		}
	}
//}
