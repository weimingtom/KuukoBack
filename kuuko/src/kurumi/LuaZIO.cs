/*
 ** $Id: lzio.c,v 1.31.1.1 2007/12/27 13:02:25 roberto Exp $
 ** a generic input stream interface
 ** See Copyright Notice in lua.h
 */
namespace kurumi
{
	public class LuaZIO
	{
		public const int EOZ = -1;			/* end of stream */

		public static int char2int(char c) 
		{ 
			return (int)c; 
		}

		public static int zgetc(ZIO z)
		{
			if (z.n-- > 0)
			{
				int ch = char2int(z.p.get(0));
				z.p.inc();
				return ch;
			}
			else
			{
				return luaZ_fill(z);
			}
		}
		
		public class Mbuffer
		{
			public CLib.CharPtr buffer = new CLib.CharPtr();
			public int n; /*uint*/
			public int buffsize; /*uint*/
		}

		public static void luaZ_initbuffer(LuaState.lua_State L, Mbuffer buff)
		{
			buff.buffer = null;
		}

		public static CLib.CharPtr luaZ_buffer(Mbuffer buff)	
		{
			return buff.buffer;
		}
		
		public static int/*uint*/ luaZ_sizebuffer(Mbuffer buff) 
		{ 
			return buff.buffsize; 
		}
		
		public static int/*uint*/ luaZ_bufflen(Mbuffer buff)	
		{
			return buff.n;
		}
		
		public static void luaZ_resetbuffer(Mbuffer buff) 
		{
			buff.n = 0;
		}

		public static void luaZ_resizebuffer(LuaState.lua_State L, Mbuffer buff, int size)
		{
			if (CLib.CharPtr.isEqual(buff.buffer, null))
			{
				buff.buffer = new CLib.CharPtr();
			}
			char[][] chars_ref = new char[1][];
			chars_ref[0] = buff.buffer.chars;
			LuaMem.luaM_reallocvector_char(L, /*ref*/ chars_ref, /*(int)*/buff.buffsize, size, new ClassType(ClassType.TYPE_CHAR));
			buff.buffer.chars = chars_ref[0];
			buff.buffsize = /*(uint)*/buff.buffer.chars.Length;
		}

		public static void luaZ_freebuffer(LuaState.lua_State L, Mbuffer buff) 
		{
			luaZ_resizebuffer(L, buff, 0);
		}

		/* --------- Private Part ------------------ */

		public class ZIO//Zio
		{
			public int n;  /*uint*/			/* bytes still unread */
			public CLib.CharPtr p;			/* current position in buffer */
			public Lua.lua_Reader reader;
			public object data;			/* additional data */
			public LuaState.lua_State L;			/* Lua state (for reader) */
			
			//public class ZIO : Zio { };
		}
	
		public static int luaZ_fill(ZIO z) 
		{
			int[]/*uint*/ size = new int[1];
			LuaState.lua_State L = z.L;
			CLib.CharPtr buff;
			LuaLimits.lua_unlock(L);
			buff = z.reader.exec(L, z.data, /*out*/ size);
			LuaLimits.lua_lock(L);
			if (CLib.CharPtr.isEqual(buff, null) || size[0] == 0) 
			{
				return EOZ;
			}
			z.n = size[0] - 1;
			z.p = new CLib.CharPtr(buff);
			int result = char2int(z.p.get(0));
			z.p.inc();
			return result;
		}

		public static int luaZ_lookahead(ZIO z) 
		{
			if (z.n == 0) 
			{
				if (luaZ_fill(z) == EOZ)
				{
					return EOZ;
				}
				else 
				{
					z.n++;  /* luaZ_fill removed first byte; put back it */
					z.p.dec();
				}
			}
			return char2int(z.p.get(0));
		}

		public static void luaZ_init(LuaState.lua_State L, ZIO z, Lua.lua_Reader reader, object data)
		{
			z.L = L;
			z.reader = reader;
			z.data = data;
			z.n = 0;
			z.p = null;
		}

		/* --------------------------------------------------------------- read --- */
		public static int/*uint*/ luaZ_read(ZIO z, CLib.CharPtr b, int/*uint*/ n) 
		{
			b = new CLib.CharPtr(b);
			while (n != 0) 
			{
				int/*uint*/ m;
				if (luaZ_lookahead(z) == EOZ)
				{
					return n;  // return number of missing bytes
				}
				m = (n <= z.n) ? n : z.n;  // min. between n and z.n
				CLib.memcpy(b, z.p, m);
				z.n -= m;
				z.p = CLib.CharPtr.plus(z.p, m);
				b = CLib.CharPtr.plus(b, m);
				n -= m;
			}
			return 0;
		}

		/* ------------------------------------------------------------------------ */
		public static CLib.CharPtr luaZ_openspace (LuaState.lua_State L, Mbuffer buff, int/*uint*/ n) 
		{
			if (n > buff.buffsize) 
			{
				if (n < LuaLimits.LUA_MINBUFFER)
				{
					n = LuaLimits.LUA_MINBUFFER;
				}
				luaZ_resizebuffer(L, buff, /*(int)*/n);
			}
			return buff.buffer;
		}
	}
}

