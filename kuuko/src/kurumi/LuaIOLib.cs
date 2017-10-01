/*
 ** $Id: liolib.c,v 2.73.1.3 2008/01/18 17:47:43 roberto Exp $
 ** Standard I/O (and system) library
 ** See Copyright Notice in lua.h
 */
namespace kurumi
{
	//using lua_Number = System.Double;
	//using lua_Integer = System.Int32;

	public class LuaIOLib
	{
		public class FilePtr
		{
	        public StreamProxy file;
		}		
		
		public const int IO_INPUT = 1;
		public const int IO_OUTPUT = 2;

		private static readonly string[] fnames = { 
			"input", 
			"output" 
		};

		private static int pushresult(LuaState.lua_State L, int i, CLib.CharPtr filename) 
		{
			int en = CLib.errno();  /* calls to Lua API may change this value */
			if (i != 0) 
			{
				LuaAPI.lua_pushboolean(L, 1);
				return 1;
			}
			else 
			{
				LuaAPI.lua_pushnil(L);
				if (CLib.CharPtr.isNotEqual(filename, null))
				{
					LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr("%s: %s"), filename, CLib.strerror(en));
				}
				else
				{
					LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr("%s"), CLib.strerror(en));
				}
				LuaAPI.lua_pushinteger(L, en);
				return 3;
			}
		}

		private static void fileerror(LuaState.lua_State L, int arg, CLib.CharPtr filename) 
		{
			LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr("%s: %s"), filename, CLib.strerror(CLib.errno()));
			LuaAuxLib.luaL_argerror(L, arg, Lua.lua_tostring(L, -1));
		}

		public static FilePtr tofilep(LuaState.lua_State L) 
		{ 
			return (FilePtr)LuaAuxLib.luaL_checkudata(L, 1, CLib.CharPtr.toCharPtr(LuaLib.LUA_FILEHANDLE)); 
		}

		private static int io_type(LuaState.lua_State L) 
		{
			object ud;
			LuaAuxLib.luaL_checkany(L, 1);
			ud = LuaAPI.lua_touserdata(L, 1);
			LuaAPI.lua_getfield(L, Lua.LUA_REGISTRYINDEX, CLib.CharPtr.toCharPtr(LuaLib.LUA_FILEHANDLE));
			if (ud == null || (LuaAPI.lua_getmetatable(L, 1) == 0) || (LuaAPI.lua_rawequal(L, -2, -1) == 0))
			{
				LuaAPI.lua_pushnil(L);  /* not a file */
			}
			else if ( (ud as FilePtr).file == null)
			{
				Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr("closed file"));
			}
			else
			{
				Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr("file"));
			}
			return 1;
		}

		private static StreamProxy tofile(LuaState.lua_State L) 
		{
			FilePtr f = tofilep(L);
			if (f.file == null)
			{
				LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("attempt to use a closed file"));
			}
			return f.file;
		}

		/*
		 ** When creating file files, always creates a `closed' file file
		 ** before opening the actual file; so, if there is a memory error, the
		 ** file is not left opened.
		 */
		private static FilePtr newfile(LuaState.lua_State L) 
		{
			FilePtr pf = (FilePtr)LuaAPI.lua_newuserdata(L, new ClassType(ClassType.TYPE_FILEPTR)); //FilePtr
			pf.file = null;  /* file file is currently `closed' */
			LuaAuxLib.luaL_getmetatable(L, CLib.CharPtr.toCharPtr(LuaLib.LUA_FILEHANDLE));
			LuaAPI.lua_setmetatable(L, -2);
			return pf;
		}

		/*
		 ** function to (not) close the standard files stdin, stdout, and stderr
		 */
		private static int io_noclose(LuaState.lua_State L) 
		{
			LuaAPI.lua_pushnil(L);
			Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr("cannot close standard file"));
			return 2;
		}

		/*
		 ** function to close 'popen' files
		 */
		private static int io_pclose(LuaState.lua_State L) 
		{
			FilePtr p = tofilep(L);
			int ok = (LuaConf.lua_pclose(L, p.file) == 0) ? 1 : 0;
			p.file = null;
			return pushresult(L, ok, null);
		}

		/*
		 ** function to close regular files
		 */
		private static int io_fclose(LuaState.lua_State L) 
		{
			FilePtr p = tofilep(L);
			int ok = (CLib.fclose(p.file) == 0) ? 1 : 0;
			p.file = null;
			return pushresult(L, ok, null);
		}

		private static int aux_close(LuaState.lua_State L) 
		{
			LuaAPI.lua_getfenv(L, 1);
			LuaAPI.lua_getfield(L, -1, CLib.CharPtr.toCharPtr("__close"));
			return (LuaAPI.lua_tocfunction(L, -1)).exec(L);
		}

		private static int io_close(LuaState.lua_State L) 
		{
			if (Lua.lua_isnone(L, 1))
			{
				LuaAPI.lua_rawgeti(L, Lua.LUA_ENVIRONINDEX, IO_OUTPUT);
			}
			tofile(L);  /* make sure argument is a file */
			return aux_close(L);
		}

		private static int io_gc(LuaState.lua_State L) 
		{
			StreamProxy f = tofilep(L).file;
			/* ignore closed files */
			if (f != null)
				aux_close(L);
			return 0;
		}

		private static int io_tostring(LuaState.lua_State L) 
		{
			StreamProxy f = tofilep(L).file;
			if (f == null)
			{
				Lua.lua_pushliteral(L, CLib.CharPtr.toCharPtr("file (closed)"));
			}
			else
			{
				LuaAPI.lua_pushfstring(L, CLib.CharPtr.toCharPtr("file (%p)"), f);
			}
			return 1;
		}

		private static int io_open(LuaState.lua_State L) 
		{
			CLib.CharPtr filename = LuaAuxLib.luaL_checkstring(L, 1);
			CLib.CharPtr mode = LuaAuxLib.luaL_optstring(L, 2, CLib.CharPtr.toCharPtr("r"));
			FilePtr pf = newfile(L);
			pf.file = CLib.fopen(filename, mode);
			return (pf.file == null) ? pushresult(L, 0, filename) : 1;
		}

		/*
		 ** this function has a separated environment, which defines the
		 ** correct __close for 'popen' files
		 */
		private static int io_popen(LuaState.lua_State L) 
		{
			CLib.CharPtr filename = LuaAuxLib.luaL_checkstring(L, 1);
			CLib.CharPtr mode = LuaAuxLib.luaL_optstring(L, 2, CLib.CharPtr.toCharPtr("r"));
			FilePtr pf = newfile(L);
			pf.file = LuaConf.lua_popen(L, filename, mode);
			return (pf.file == null) ? pushresult(L, 0, filename) : 1;
		}

		private static int io_tmpfile(LuaState.lua_State L) 
		{
			FilePtr pf = newfile(L);
			pf.file = CLib.tmpfile();
			return (pf.file == null) ? pushresult(L, 0, null) : 1;
		}

		private static StreamProxy getiofile(LuaState.lua_State L, int findex) 
		{
			StreamProxy f;
			LuaAPI.lua_rawgeti(L, Lua.LUA_ENVIRONINDEX, findex);
			f = (LuaAPI.lua_touserdata(L, -1) as FilePtr).file;
			if (f == null)
			{
				LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("standard %s file is closed"), fnames[findex - 1]);
			}
			return f;
		}

		private static int g_iofile(LuaState.lua_State L, int f, CLib.CharPtr mode) 
		{
			if (!Lua.lua_isnoneornil(L, 1))
			{
				CLib.CharPtr filename = Lua.lua_tostring(L, 1);
				if (CLib.CharPtr.isNotEqual(filename, null)) 
				{
					FilePtr pf = newfile(L);
					pf.file = CLib.fopen(filename, mode);
					if (pf.file == null)
					{
						fileerror(L, 1, filename);
					}
				}
				else 
				{
					tofile(L);  /* check that it's a valid file file */
					LuaAPI.lua_pushvalue(L, 1);
				}
				LuaAPI.lua_rawseti(L, Lua.LUA_ENVIRONINDEX, f);
			}
			/* return current value */
			LuaAPI.lua_rawgeti(L, Lua.LUA_ENVIRONINDEX, f);
			return 1;
		}

		private static int io_input(LuaState.lua_State L) 
		{
			return g_iofile(L, IO_INPUT, CLib.CharPtr.toCharPtr("r"));
		}

		private static int io_output(LuaState.lua_State L) 
		{
			return g_iofile(L, IO_OUTPUT, CLib.CharPtr.toCharPtr("w"));
		}

		private static void aux_lines(LuaState.lua_State L, int idx, int toclose) 
		{
			LuaAPI.lua_pushvalue(L, idx);
			LuaAPI.lua_pushboolean(L, toclose);  /* close/not close file when finished */
			LuaAPI.lua_pushcclosure(L, new LuaIOLib_delegate("io_readline"), 2);
		}

		private static int f_lines(LuaState.lua_State L) 
		{
			tofile(L);  /* check that it's a valid file file */
			aux_lines(L, 1, 0);
			return 1;
		}

		private static int io_lines(LuaState.lua_State L) 
		{
			if (Lua.lua_isnoneornil(L, 1))
			{  
				/* no arguments? */
				/* will iterate over default input */
				LuaAPI.lua_rawgeti(L, Lua.LUA_ENVIRONINDEX, IO_INPUT);
				return f_lines(L);
			}
			else 
			{
				CLib.CharPtr filename = LuaAuxLib.luaL_checkstring(L, 1);
				FilePtr pf = newfile(L);
				pf.file = CLib.fopen(filename, CLib.CharPtr.toCharPtr("r"));
				if (pf.file == null)
				{
					fileerror(L, 1, filename);
				}
				aux_lines(L, LuaAPI.lua_gettop(L), 1);
				return 1;
			}
		}


		/*
		 ** {======================================================
		 ** READ
		 ** =======================================================
		 */

		private static int read_number(LuaState.lua_State L, StreamProxy f) 
		{
			//lua_Number d;
			object[] parms = {(object)(double)0.0};
			if (CLib.fscanf(f, CLib.CharPtr.toCharPtr(LuaConf.LUA_NUMBER_SCAN), parms) == 1)
			{
				LuaAPI.lua_pushnumber(L, (double)parms[0]);
				return 1;
			}
			else 
			{
				return 0;  /* read fails */
			}
		}

		private static int test_eof(LuaState.lua_State L, StreamProxy f) 
		{
			int c = CLib.getc(f);
			CLib.ungetc(c, f);
			LuaAPI.lua_pushlstring(L, null, 0);
			return (c != CLib.EOF) ? 1 : 0;
		}

		private static int read_line(LuaState.lua_State L, StreamProxy f) 
		{
			LuaAuxLib.luaL_Buffer b = new LuaAuxLib.luaL_Buffer();
			LuaAuxLib.luaL_buffinit(L, b);
			for (;;) 
			{
				int/*uint*/ l;
				CLib.CharPtr p = LuaAuxLib.luaL_prepbuffer(b);
				if (CLib.CharPtr.isEqual(CLib.fgets(p, f), null))
				{  
					/* eof? */
					LuaAuxLib.luaL_pushresult(b);  /* close buffer */
					return (LuaAPI.lua_objlen(L, -1) > 0) ? 1 : 0;  /* check whether read something */
				}
				l = (int/*uint*/)CLib.strlen(p);
				if (l == 0 || p.get(l - 1) != '\n')
				{
					LuaAuxLib.luaL_addsize(b, (int)l);
				}
				else 
				{
					LuaAuxLib.luaL_addsize(b, (int)(l - 1));  /* do not include `eol' */
					LuaAuxLib.luaL_pushresult(b);  /* close buffer */
					return 1;  /* read at least an `eol' */
				}
			}
		}

		private static int read_chars(LuaState.lua_State L, StreamProxy f, long/*uint*/ n) 
		{
			long/*uint*/ rlen;  /* how much to read */
			int/*uint*/ nr;  /* number of chars actually read */
			LuaAuxLib.luaL_Buffer b = new LuaAuxLib.luaL_Buffer();
			LuaAuxLib.luaL_buffinit(L, b);
			rlen = LuaConf.LUAL_BUFFERSIZE;  /* try to read that much each time */
			do 
			{
				CLib.CharPtr p = LuaAuxLib.luaL_prepbuffer(b);
				if (rlen > n) 
				{
					rlen = n;  /* cannot read more than asked */
				}
                nr = (int/*uint*/)CLib.fread(p, CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_CHAR)), (int)rlen, f); //typeof(char)
				LuaAuxLib.luaL_addsize(b, (int)nr);
				n -= nr;  /* still have to read `n' chars */
			} while (n > 0 && nr == rlen);  /* until end of count or eof */
			LuaAuxLib.luaL_pushresult(b);  /* close buffer */
			return (n == 0 || LuaAPI.lua_objlen(L, -1) > 0) ? 1 : 0;
		}

		private static int g_read(LuaState.lua_State L, StreamProxy f, int first) 
		{
			int nargs = LuaAPI.lua_gettop(L) - 1;
			int success;
			int n;
			CLib.clearerr(f);
			if (nargs == 0) 
			{  
				/* no arguments? */
				success = read_line(L, f);
				n = first + 1;  /* to return 1 result */
			}
			else 
			{  
				/* ensure stack space for all results and for auxlib's buffer */
				LuaAuxLib.luaL_checkstack(L, nargs + Lua.LUA_MINSTACK, CLib.CharPtr.toCharPtr("too many arguments"));
				success = 1;
				for (n = first; (nargs-- != 0) && (success != 0); n++) 
				{
					if (LuaAPI.lua_type(L, n) == Lua.LUA_TNUMBER)
					{
						int/*uint*/ l = (int/*uint*/)LuaAPI.lua_tointeger(L, n);
						success = (l == 0) ? test_eof(L, f) : read_chars(L, f, l);
					}
					else 
					{
						CLib.CharPtr p = Lua.lua_tostring(L, n);
						LuaAuxLib.luaL_argcheck(L, (CLib.CharPtr.isNotEqual(p, null)) && (p.get(0) == '*'), n, "invalid option");
						switch (p.get(1)) 
						{
							case 'n':  /* number */
								{
									success = read_number(L, f);
									break;
								}
							case 'l':  /* line */
								{
									success = read_line(L, f);
									break;
								}
							case 'a':  /* file */
								{
								read_chars(L, f, /*~((uint)0*/ (long)((~(int)0) & 0xffffffff) );  /* read MAX_uint chars */
									success = 1; /* always success */
									break;
								}
							default:
								{
									return LuaAuxLib.luaL_argerror(L, n, CLib.CharPtr.toCharPtr("invalid format"));
								}
						}
					}
				}
			}
			if (CLib.ferror(f) != 0)
			{
				return pushresult(L, 0, null);
			}
			if (success == 0) 
			{
				Lua.lua_pop(L, 1);  /* remove last result */
				LuaAPI.lua_pushnil(L);  /* push nil instead */
			}
			return n - first;
		}

		private static int io_read(LuaState.lua_State L) 
		{
			return g_read(L, getiofile(L, IO_INPUT), 1);
		}

		private static int f_read(LuaState.lua_State L) 
		{
			return g_read(L, tofile(L), 2);
		}

		private static int io_readline(LuaState.lua_State L) 
		{
			StreamProxy f = (LuaAPI.lua_touserdata(L, Lua.lua_upvalueindex(1)) as FilePtr).file;
			int sucess;
			if (f == null)  /* file is already closed? */
			{
				LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("file is already closed"));
			}
			sucess = read_line(L, f);
			if (CLib.ferror(f) != 0)
			{
				return LuaAuxLib.luaL_error(L, CLib.CharPtr.toCharPtr("%s"), CLib.strerror(CLib.errno()));
			}
			if (sucess != 0) 
			{
				return 1;
			}
			else 
			{  
				/* EOF */
				if (LuaAPI.lua_toboolean(L, Lua.lua_upvalueindex(2)) != 0)
				{  
					/* generator created file? */
					LuaAPI.lua_settop(L, 0);
					LuaAPI.lua_pushvalue(L, Lua.lua_upvalueindex(1));
					aux_close(L);  /* close it */
				}
				return 0;
			}
		}

		/* }====================================================== */

		private static int g_write (LuaState.lua_State L, StreamProxy f, int arg) 
		{
			int nargs = LuaAPI.lua_gettop(L) - 1;
			int status = 1;
			for (; (nargs--) != 0; arg++) 
			{
				if (LuaAPI.lua_type(L, arg) == Lua.LUA_TNUMBER)
				{
					/* optimization: could be done exactly as for strings */
					status = ((status!=0) &&
					          (CLib.fprintf(f, CLib.CharPtr.toCharPtr(LuaConf.LUA_NUMBER_FMT), LuaAPI.lua_tonumber(L, arg)) > 0)) ? 1 : 0;
				}
				else 
				{
					int[]/*uint*/ l = new int[1];
					CLib.CharPtr s = LuaAuxLib.luaL_checklstring(L, arg, /*out*/ l);
                    status = ((status != 0) && (CLib.fwrite(s, CLib.GetUnmanagedSize(new ClassType(ClassType.TYPE_CHAR)), (int)l[0], f) == l[0])) ? 1 : 0; //typeof(char)
				}
			}
			return pushresult(L, status, null);
		}

		private static int io_write(LuaState.lua_State L) 
		{
			return g_write(L, getiofile(L, IO_OUTPUT), 1);
		}

		private static int f_write(LuaState.lua_State L) 
		{
			return g_write(L, tofile(L), 2);
		}

		private static int f_seek(LuaState.lua_State L) 
		{
			int[] mode = { 
				CLib.SEEK_SET, 
				CLib.SEEK_CUR, 
				CLib.SEEK_END 
			};
			CLib.CharPtr[] modenames = { 
				CLib.CharPtr.toCharPtr("set"), 
				CLib.CharPtr.toCharPtr("cur"), 
				CLib.CharPtr.toCharPtr("end"), 
				null
			};
			StreamProxy f = tofile(L);
			int op = LuaAuxLib.luaL_checkoption(L, 2, CLib.CharPtr.toCharPtr("cur"), modenames);
			long offset = LuaAuxLib.luaL_optlong(L, 3, 0);
			op = CLib.fseek(f, offset, mode[op]);
			if (op != 0)
			{
				return pushresult(L, 0, null);  /* error */
			}
			else 
			{
				LuaAPI.lua_pushinteger(L, CLib.ftell(f));
				return 1;
			}
		}

		private static int f_setvbuf(LuaState.lua_State L) 
		{
			CLib.CharPtr[] modenames = { 
				CLib.CharPtr.toCharPtr("no"), 
				CLib.CharPtr.toCharPtr("full"), 
				CLib.CharPtr.toCharPtr("line"), 
				null 
			};
			int[] mode = { CLib._IONBF, CLib._IOFBF, CLib._IOLBF };
			StreamProxy f = tofile(L);
			int op = LuaAuxLib.luaL_checkoption(L, 2, null, modenames);
			int/*Int32*//*lua_Integer*/ sz = LuaAuxLib.luaL_optinteger(L, 3, LuaConf.LUAL_BUFFERSIZE);
			int res = CLib.setvbuf(f, null, mode[op], (/*uint*/int)sz);
			return pushresult(L, (res == 0) ? 1 : 0, null);
		}
		
		private static int io_flush(LuaState.lua_State L) 
		{
			int result = 1;
			try 
			{
				getiofile(L, IO_OUTPUT).Flush();
			} 
			catch 
			{
				result = 0;
			}
			return pushresult(L, result, null);
		}

		private static int f_flush (LuaState.lua_State L) 
		{
			int result = 1;
			try 
			{
				tofile(L).Flush();
			} 
			catch 
			{
				result = 0;
			}
			return pushresult(L, result, null);
		}


		private readonly static LuaAuxLib.luaL_Reg[] iolib = {
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("close"), new LuaIOLib_delegate("io_close")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("flush"), new LuaIOLib_delegate("io_flush")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("input"), new LuaIOLib_delegate("io_input")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("lines"), new LuaIOLib_delegate("io_lines")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("open"), new LuaIOLib_delegate("io_open")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("output"), new LuaIOLib_delegate("io_output")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("popen"), new LuaIOLib_delegate("io_popen")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("read"), new LuaIOLib_delegate("io_read")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("tmpfile"), new LuaIOLib_delegate("io_tmpfile")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("type"), new LuaIOLib_delegate("io_type")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("write"), new LuaIOLib_delegate("io_write")),
			new LuaAuxLib.luaL_Reg(null, null)
		};

		private readonly static LuaAuxLib.luaL_Reg[] flib = {
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("close"), new LuaIOLib_delegate("io_close")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("flush"), new LuaIOLib_delegate("f_flush")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("lines"), new LuaIOLib_delegate("f_lines")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("read"), new LuaIOLib_delegate("f_read")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("seek"), new LuaIOLib_delegate("f_seek")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("setvbuf"), new LuaIOLib_delegate("f_setvbuf")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("write"), new LuaIOLib_delegate("f_write")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("__gc"), new LuaIOLib_delegate("io_gc")),
			new LuaAuxLib.luaL_Reg(CLib.CharPtr.toCharPtr("__tostring"), new LuaIOLib_delegate("io_tostring")),
			new LuaAuxLib.luaL_Reg(null, null)
		};
		
		public class LuaIOLib_delegate : Lua.lua_CFunction
		{
			private string name;
			
			public LuaIOLib_delegate(string name)
			{
				this.name = name;
			}
			
			public int exec(LuaState.lua_State L)
			{
				if ("io_close".Equals(name))
				{
					return io_close(L);
				} 
				else if ("io_flush".Equals(name)) 
				{
					return io_flush(L);
				} 
				else if ("io_input".Equals(name)) 
				{
					return io_input(L);
				} 
				else if ("io_lines".Equals(name)) 
				{
				    return io_lines(L);
				}
				else if ("io_open".Equals(name))
				{
				    return io_open(L);
				}
				else if ("io_output".Equals(name))
				{
					return io_output(L);
				}
				else if ("io_popen".Equals(name))
				{
					return io_popen(L);
				}
				else if ("io_read".Equals(name))
				{
					return io_read(L);
				}
				else if ("io_tmpfile".Equals(name))
				{
					return io_tmpfile(L);
				}
				else if ("io_type".Equals(name))
				{
					return io_type(L);
				}
				else if ("io_write".Equals(name))
				{
					return io_write(L);
				}
				else if ("f_flush".Equals(name))
				{
					return f_flush(L);
				}
				else if ("f_lines".Equals(name))
				{
					return f_lines(L);
				}
				else if ("f_read".Equals(name))
				{
					return f_read(L);
				}
				else if ("f_seek".Equals(name))
				{
					return f_seek(L);
				}
				else if ("f_setvbuf".Equals(name))
				{
					return f_setvbuf(L);
				}
				else if ("f_write".Equals(name))
				{
					return f_write(L);
				}
				else if ("io_gc".Equals(name))
				{
					return io_gc(L);
				}
				else if ("io_tostring".Equals(name))
		        {
		         	return io_tostring(L);
		        }
				else if ("io_fclose".Equals(name))
				{
					return io_fclose(L);
				}
				else if ("io_noclose".Equals(name))
				{
					return io_noclose(L);
				}
				else if ("io_pclose".Equals(name))
				{
					return io_pclose(L);
				}
				else if ("io_readline".Equals(name))
				{
					return io_readline(L);
				}
				else
				{
					return 0;
				}
			}
		}

		private static void createmeta(LuaState.lua_State L) 
		{
			LuaAuxLib.luaL_newmetatable(L, CLib.CharPtr.toCharPtr(LuaLib.LUA_FILEHANDLE));  /* create metatable for file files */
			LuaAPI.lua_pushvalue(L, -1);  /* push metatable */
			LuaAPI.lua_setfield(L, -2, CLib.CharPtr.toCharPtr("__index"));  /* metatable.__index = metatable */
			LuaAuxLib.luaL_register(L, null, flib);  /* file methods */
		}

		private static void createstdfile(LuaState.lua_State L, StreamProxy f, int k, CLib.CharPtr fname) 
		{
			newfile(L).file = f;
			if (k > 0) 
			{
				LuaAPI.lua_pushvalue(L, -1);
				LuaAPI.lua_rawseti(L, Lua.LUA_ENVIRONINDEX, k);
			}
			LuaAPI.lua_pushvalue(L, -2);  /* copy environment */
			LuaAPI.lua_setfenv(L, -2);  /* set it */
			LuaAPI.lua_setfield(L, -3, fname);
		}

		private static void newfenv(LuaState.lua_State L, Lua.lua_CFunction cls) 
		{
			LuaAPI.lua_createtable(L, 0, 1);
			Lua.lua_pushcfunction(L, cls);
			LuaAPI.lua_setfield(L, -2, CLib.CharPtr.toCharPtr("__close"));
		}

		public static int luaopen_io(LuaState.lua_State L) 
		{
			createmeta(L);
			/* create (private) environment (with fields IO_INPUT, IO_OUTPUT, __close) */
			newfenv(L, new LuaIOLib_delegate("io_fclose"));
			LuaAPI.lua_replace(L, Lua.LUA_ENVIRONINDEX);
			/* open library */
			LuaAuxLib.luaL_register(L, CLib.CharPtr.toCharPtr(LuaLib.LUA_IOLIBNAME), iolib);
			/* create (and set) default files */
			newfenv(L, new LuaIOLib_delegate("io_noclose"));  /* close function for default files */
			createstdfile(L, CLib.stdin, IO_INPUT, CLib.CharPtr.toCharPtr("stdin"));
			createstdfile(L, CLib.stdout, IO_OUTPUT, CLib.CharPtr.toCharPtr("stdout"));
			createstdfile(L, CLib.stderr, 0, CLib.CharPtr.toCharPtr("stderr"));
			Lua.lua_pop(L, 1);  /* pop environment for default files */
			LuaAPI.lua_getfield(L, -1, CLib.CharPtr.toCharPtr("popen"));
			newfenv(L, new LuaIOLib_delegate("io_pclose"));  /* create environment for 'popen' */
			LuaAPI.lua_setfenv(L, -2);  /* set fenv for 'popen' */
			Lua.lua_pop(L, 1);  /* pop 'popen' */
			return 1;
		}
	}
}
