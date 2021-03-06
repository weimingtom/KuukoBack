/*
 ** $Id: lmem.c,v 1.70.1.1 2007/12/27 13:02:25 roberto Exp $
 ** Interface to Memory Manager
 ** See Copyright Notice in lua.h
 */
using System;

namespace kurumi
{
	public class LuaMem
	{
		public const string MEMERRMSG = "not enough memory";

        //-------------------------------

        public static char[] luaM_reallocv_char(LuaState.lua_State L, char[] block, int new_size, ClassType t)
        {
            return (char[])luaM_realloc__char(L, block, new_size, t);
        }

        public static LuaObject.TValue[] luaM_reallocv_TValue(LuaState.lua_State L, LuaObject.TValue[] block, int new_size, ClassType t)
        {
            return (LuaObject.TValue[])luaM_realloc__TValue(L, block, new_size, t);
        }

        public static LuaObject.TString[] luaM_reallocv_TString(LuaState.lua_State L, LuaObject.TString[] block, int new_size, ClassType t)
        {
            return (LuaObject.TString[])luaM_realloc__TString(L, block, new_size, t);
        }

        public static LuaState.CallInfo[] luaM_reallocv_CallInfo(LuaState.lua_State L, LuaState.CallInfo[] block, int new_size, ClassType t)
        {
            return (LuaState.CallInfo[])luaM_realloc__CallInfo(L, block, new_size, t);
        }

        public static long[] luaM_reallocv_long(LuaState.lua_State L, long[] block, int new_size, ClassType t)
        {
            return (long[])luaM_realloc__long(L, block, new_size, t);
        }

        public static int[] luaM_reallocv_int(LuaState.lua_State L, int[] block, int new_size, ClassType t)
        {
            return (int[])luaM_realloc__int(L, block, new_size, t);
        }

        public static LuaObject.Proto[] luaM_reallocv_Proto(LuaState.lua_State L, LuaObject.Proto[] block, int new_size, ClassType t)
        {
            return (LuaObject.Proto[])luaM_realloc__Proto(L, block, new_size, t);
        }

        public static LuaObject.LocVar[] luaM_reallocv_LocVar(LuaState.lua_State L, LuaObject.LocVar[] block, int new_size, ClassType t)
        {
            return (LuaObject.LocVar[])luaM_realloc__LocVar(L, block, new_size, t);
        }

        public static LuaObject.Node[] luaM_reallocv_Node(LuaState.lua_State L, LuaObject.Node[] block, int new_size, ClassType t)
        {
            return (LuaObject.Node[])luaM_realloc__Node(L, block, new_size, t);
        }

        public static LuaState.GCObject[] luaM_reallocv_GCObject(LuaState.lua_State L, LuaState.GCObject[] block, int new_size, ClassType t)
        {
            return (LuaState.GCObject[])luaM_realloc__GCObject(L, block, new_size, t);
        }

        //-------------------------------

		//#define luaM_freemem(L, b, s)	luaM_realloc_(L, (b), (s), 0)
		//#define luaM_free(L, b)		luaM_realloc_(L, (b), sizeof(*(b)), 0)
		//public static void luaM_freearray(lua_State L, object b, int n, Type t) { luaM_reallocv(L, b, n, 0, Marshal.SizeOf(b)); }

		// C# has it's own gc, so nothing to do here...in theory...
		public static void luaM_freemem_Udata(LuaState.lua_State L, LuaObject.Udata b, ClassType t) 
		{ 
			luaM_realloc__Udata(L, new LuaObject.Udata[] {b}, 0, t); 
		}

        public static void luaM_freemem_TString(LuaState.lua_State L, LuaObject.TString b, ClassType t)
        {
            luaM_realloc__TString(L, new LuaObject.TString[] { b }, 0, t);
        }

        //-------------------------------

		public static void luaM_free_Table(LuaState.lua_State L, LuaObject.Table b, ClassType t) 
		{ 
			luaM_realloc__Table(L, new LuaObject.Table[] {b}, 0, t); 
		}

        public static void luaM_free_UpVal(LuaState.lua_State L, LuaObject.UpVal b, ClassType t)
        {
            luaM_realloc__UpVal(L, new LuaObject.UpVal[] { b }, 0, t);
        }

        public static void luaM_free_Proto(LuaState.lua_State L, LuaObject.Proto b, ClassType t)
        {
            luaM_realloc__Proto(L, new LuaObject.Proto[] { b }, 0, t);
        }

        //-------------------------------

		public static void luaM_freearray_long(LuaState.lua_State L, long[] b, ClassType t) 
		{ 
			luaM_reallocv_long(L, b, 0, t); 
		}

        public static void luaM_freearray_Proto(LuaState.lua_State L, LuaObject.Proto[] b, ClassType t)
        {
            luaM_reallocv_Proto(L, b, 0, t);
        }

        public static void luaM_freearray_TValue(LuaState.lua_State L, LuaObject.TValue[] b, ClassType t)
        {
            luaM_reallocv_TValue(L, b, 0, t);
        }

        public static void luaM_freearray_int(LuaState.lua_State L, int[] b, ClassType t)
        {
            luaM_reallocv_int(L, b, 0, t);
        }

        public static void luaM_freearray_LocVar(LuaState.lua_State L, LuaObject.LocVar[] b, ClassType t)
        {
            luaM_reallocv_LocVar(L, b, 0, t);
        }

        public static void luaM_freearray_TString(LuaState.lua_State L, LuaObject.TString[] b, ClassType t)
        {
            luaM_reallocv_TString(L, b, 0, t);
        }

        public static void luaM_freearray_Node(LuaState.lua_State L, LuaObject.Node[] b, ClassType t)
        {
            luaM_reallocv_Node(L, b, 0, t);
        }

        public static void luaM_freearray_CallInfo(LuaState.lua_State L, LuaState.CallInfo[] b, ClassType t)
        {
            luaM_reallocv_CallInfo(L, b, 0, t);
        }

        public static void luaM_freearray_GCObject(LuaState.lua_State L, LuaState.GCObject[] b, ClassType t)
        {
            luaM_reallocv_GCObject(L, b, 0, t);
        }

        //-------------------------------


		//public static T luaM_malloc<T>(lua_State L, ClassType t) 
		//{ 
		//	return (T)luaM_realloc_<T>(L, t); 
		//}

        public static LuaObject.Proto luaM_new_Proto(LuaState.lua_State L, ClassType t) 
		{ 
			return (LuaObject.Proto)luaM_realloc__Proto(L, t); 
		}

        public static LuaObject.Closure luaM_new_Closure(LuaState.lua_State L, ClassType t)
        {
            return (LuaObject.Closure)luaM_realloc__Closure(L, t);
        }

        public static LuaObject.UpVal luaM_new_UpVal(LuaState.lua_State L, ClassType t)
        {
            return (LuaObject.UpVal)luaM_realloc__UpVal(L, t);
        }

        public static LuaState.lua_State luaM_new_lua_State(LuaState.lua_State L, ClassType t)
        {
            return (LuaState.lua_State)luaM_realloc__lua_State(L, t);
        }

        public static LuaObject.Table luaM_new_Table(LuaState.lua_State L, ClassType t)
        {
            return (LuaObject.Table)luaM_realloc__Table(L, t);
        }


        //-------------------------------

		public static long[] luaM_newvector_long(LuaState.lua_State L, int n, ClassType t)
		{
			return luaM_reallocv_long(L, null, n, t);
		}

        public static LuaObject.TString[] luaM_newvector_TString(LuaState.lua_State L, int n, ClassType t)
		{
            return luaM_reallocv_TString(L, null, n, t);
		}

        public static LuaObject.LocVar[] luaM_newvector_LocVar(LuaState.lua_State L, int n, ClassType t)
        {
            return luaM_reallocv_LocVar(L, null, n, t);
        }

        public static int[] luaM_newvector_int(LuaState.lua_State L, int n, ClassType t)
        {
            return luaM_reallocv_int(L, null, n, t);
        }

        public static LuaObject.Proto[] luaM_newvector_Proto(LuaState.lua_State L, int n, ClassType t)
        {
            return luaM_reallocv_Proto(L, null, n, t);
        }

        public static LuaObject.TValue[] luaM_newvector_TValue(LuaState.lua_State L, int n, ClassType t)
        {
            return luaM_reallocv_TValue(L, null, n, t);
        }

        public static LuaState.CallInfo[] luaM_newvector_CallInfo(LuaState.lua_State L, int n, ClassType t)
        {
            return luaM_reallocv_CallInfo(L, null, n, t);
        }

        public static LuaObject.Node[] luaM_newvector_Node(LuaState.lua_State L, int n, ClassType t)
        {
            return luaM_reallocv_Node(L, null, n, t);
        }

        //-------------------------------







		public static void luaM_growvector_long(LuaState.lua_State L, /*ref*/ long[][] v, int nelems, /*ref*/ int[] size, int limit, CLib.CharPtr e, ClassType t)
		{
			if (nelems + 1 > size[0])
			{
				v[0] = (long[])luaM_growaux__long(L, /*ref*/ v, /*ref*/ size, limit, e, t);
			}
		}

        public static void luaM_growvector_Proto(LuaState.lua_State L, /*ref*/ LuaObject.Proto[][] v, int nelems, /*ref*/ int[] size, int limit, CLib.CharPtr e, ClassType t)
		{
			if (nelems + 1 > size[0])
			{
				v[0] = (LuaObject.Proto[])luaM_growaux__Proto(L, /*ref*/ v, /*ref*/ size, limit, e, t);
			}
		}

        public static void luaM_growvector_TString(LuaState.lua_State L, /*ref*/ LuaObject.TString[][] v, int nelems, /*ref*/ int[] size, int limit, CLib.CharPtr e, ClassType t)
        {
            if (nelems + 1 > size[0])
            {
                v[0] = (LuaObject.TString[])luaM_growaux__TString(L, /*ref*/ v, /*ref*/ size, limit, e, t);
            }
        }

        public static void luaM_growvector_TValue(LuaState.lua_State L, /*ref*/ LuaObject.TValue[][] v, int nelems, /*ref*/ int[] size, int limit, CLib.CharPtr e, ClassType t)
        {
            if (nelems + 1 > size[0])
            {
                v[0] = (LuaObject.TValue[])luaM_growaux__TValue(L, /*ref*/ v, /*ref*/ size, limit, e, t);
            }
        }

        public static void luaM_growvector_LocVar(LuaState.lua_State L, /*ref*/ LuaObject.LocVar[][] v, int nelems, /*ref*/ int[] size, int limit, CLib.CharPtr e, ClassType t)
        {
            if (nelems + 1 > size[0])
            {
                v[0] = (LuaObject.LocVar[])luaM_growaux__LocVar(L, /*ref*/ v, /*ref*/ size, limit, e, t);
            }
        }

        public static void luaM_growvector_int(LuaState.lua_State L, /*ref*/ int[][] v, int nelems, /*ref*/ int[] size, int limit, CLib.CharPtr e, ClassType t)
        {
            if (nelems + 1 > size[0])
            {
                v[0] = (int[])luaM_growaux__int(L, /*ref*/ v, /*ref*/ size, limit, e, t);
            }
        }

        //-------------------------------



		public static char[] luaM_reallocvector_char(LuaState.lua_State L, /*ref*/ char[][] v, int oldn, int n, ClassType t)
		{
			ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
			v[0] = luaM_reallocv_char(L, v[0], n, t);
			return v[0];
		}

        public static LuaObject.TValue[] luaM_reallocvector_TValue(LuaState.lua_State L, /*ref*/ LuaObject.TValue[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_TValue(L, v[0], n, t);
            return v[0];
        }

        public static LuaObject.TString[] luaM_reallocvector_TString(LuaState.lua_State L, /*ref*/ LuaObject.TString[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_TString(L, v[0], n, t);
            return v[0];
        }

        public static LuaState.CallInfo[] luaM_reallocvector_CallInfo(LuaState.lua_State L, /*ref*/ LuaState.CallInfo[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_CallInfo(L, v[0], n, t);
            return v[0];
        }

        public static long[] luaM_reallocvector_long(LuaState.lua_State L, /*ref*/ long[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_long(L, v[0], n, t);
            return v[0];
        }

        public static int[] luaM_reallocvector_int(LuaState.lua_State L, /*ref*/ int[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_int(L, v[0], n, t);
            return v[0];
        }

        public static LuaObject.Proto[] luaM_reallocvector_Proto(LuaState.lua_State L, /*ref*/ LuaObject.Proto[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_Proto(L, v[0], n, t);
            return v[0];
        }

        public static LuaObject.LocVar[] luaM_reallocvector_LocVar(LuaState.lua_State L, /*ref*/ LuaObject.LocVar[][] v, int oldn, int n, ClassType t)
        {
            ClassType.Assert((v[0] == null && oldn == 0) || (v[0].Length == oldn));
            v[0] = luaM_reallocv_LocVar(L, v[0], n, t);
            return v[0];
        }

        //-------------------------------


		/*
		 ** About the realloc function:
		 ** void * frealloc (void *ud, void *ptr, uint osize, uint nsize);
		 ** (`osize' is the old size, `nsize' is the new size)
		 **
		 ** Lua ensures that (ptr == null) iff (osize == 0).
		 **
		 ** * frealloc(ud, null, 0, x) creates a new block of size `x'
		 **
		 ** * frealloc(ud, p, x, 0) frees the block `p'
		 ** (in this specific case, frealloc must return null).
		 ** particularly, frealloc(ud, null, 0, 0) does nothing
		 ** (which is equivalent to free(null) in ANSI C)
		 **
		 ** frealloc returns null if it cannot create or reallocate the area
		 ** (any reallocation to an equal or smaller size cannot fail!)
		 */
		public const int MINSIZEARRAY = 4;


		public static long[] luaM_growaux__long(LuaState.lua_State L, /*ref*/ long[][] block, /*ref*/ int[] size,
			int limit, CLib.CharPtr errormsg, ClassType t)
		{
			long[] newblock;
			int newsize;
			if (size[0] >= limit / 2)
			{  
				/* cannot double it? */
				if (size[0] >= limit)  /* cannot grow even a little? */
				{
					LuaDebug.luaG_runerror(L, errormsg);
				}
				newsize = limit;  /* still have at least one free place */
			}
			else
			{
				newsize = size[0] * 2;
				if (newsize < MINSIZEARRAY)
				{
					newsize = MINSIZEARRAY;  /* minimum size */
				}
			}
			newblock = luaM_reallocv_long(L, block[0], newsize, t);
			size[0] = newsize;  /* update only when everything else is OK */
			return newblock;
		}

        public static LuaObject.Proto[] luaM_growaux__Proto(LuaState.lua_State L, /*ref*/ LuaObject.Proto[][] block, /*ref*/ int[] size,
                    int limit, CLib.CharPtr errormsg, ClassType t)
        {
            LuaObject.Proto[] newblock;
            int newsize;
            if (size[0] >= limit / 2)
            {
                /* cannot double it? */
                if (size[0] >= limit)  /* cannot grow even a little? */
                {
                    LuaDebug.luaG_runerror(L, errormsg);
                }
                newsize = limit;  /* still have at least one free place */
            }
            else
            {
                newsize = size[0] * 2;
                if (newsize < MINSIZEARRAY)
                {
                    newsize = MINSIZEARRAY;  /* minimum size */
                }
            }
            newblock = luaM_reallocv_Proto(L, block[0], newsize, t);
            size[0] = newsize;  /* update only when everything else is OK */
            return newblock;
        }

        public static LuaObject.TString[] luaM_growaux__TString(LuaState.lua_State L, /*ref*/ LuaObject.TString[][] block, /*ref*/ int[] size,
                            int limit, CLib.CharPtr errormsg, ClassType t)
        {
            LuaObject.TString[] newblock;
            int newsize;
            if (size[0] >= limit / 2)
            {
                /* cannot double it? */
                if (size[0] >= limit)  /* cannot grow even a little? */
                {
                    LuaDebug.luaG_runerror(L, errormsg);
                }
                newsize = limit;  /* still have at least one free place */
            }
            else
            {
                newsize = size[0] * 2;
                if (newsize < MINSIZEARRAY)
                {
                    newsize = MINSIZEARRAY;  /* minimum size */
                }
            }
            newblock = luaM_reallocv_TString(L, block[0], newsize, t);
            size[0] = newsize;  /* update only when everything else is OK */
            return newblock;
        }

        public static LuaObject.TValue[] luaM_growaux__TValue(LuaState.lua_State L, /*ref*/ LuaObject.TValue[][] block, /*ref*/ int[] size,
                            int limit, CLib.CharPtr errormsg, ClassType t)
        {
            LuaObject.TValue[] newblock;
            int newsize;
            if (size[0] >= limit / 2)
            {
                /* cannot double it? */
                if (size[0] >= limit)  /* cannot grow even a little? */
                {
                    LuaDebug.luaG_runerror(L, errormsg);
                }
                newsize = limit;  /* still have at least one free place */
            }
            else
            {
                newsize = size[0] * 2;
                if (newsize < MINSIZEARRAY)
                {
                    newsize = MINSIZEARRAY;  /* minimum size */
                }
            }
            newblock = luaM_reallocv_TValue(L, block[0], newsize, t);
            size[0] = newsize;  /* update only when everything else is OK */
            return newblock;
        }

        public static LuaObject.LocVar[] luaM_growaux__LocVar(LuaState.lua_State L, /*ref*/ LuaObject.LocVar[][] block, /*ref*/ int[] size,
                              int limit, CLib.CharPtr errormsg, ClassType t)
        {
            LuaObject.LocVar[] newblock;
            int newsize;
            if (size[0] >= limit / 2)
            {
                /* cannot double it? */
                if (size[0] >= limit)  /* cannot grow even a little? */
                {
                    LuaDebug.luaG_runerror(L, errormsg);
                }
                newsize = limit;  /* still have at least one free place */
            }
            else
            {
                newsize = size[0] * 2;
                if (newsize < MINSIZEARRAY)
                {
                    newsize = MINSIZEARRAY;  /* minimum size */
                }
            }
            newblock = luaM_reallocv_LocVar(L, block[0], newsize, t);
            size[0] = newsize;  /* update only when everything else is OK */
            return newblock;
        }

        public static int[] luaM_growaux__int(LuaState.lua_State L, /*ref*/ int[][] block, /*ref*/ int[] size,
                              int limit, CLib.CharPtr errormsg, ClassType t)
        {
            int[] newblock;
            int newsize;
            if (size[0] >= limit / 2)
            {
                /* cannot double it? */
                if (size[0] >= limit)  /* cannot grow even a little? */
                {
                    LuaDebug.luaG_runerror(L, errormsg);
                }
                newsize = limit;  /* still have at least one free place */
            }
            else
            {
                newsize = size[0] * 2;
                if (newsize < MINSIZEARRAY)
                {
                    newsize = MINSIZEARRAY;  /* minimum size */
                }
            }
            newblock = luaM_reallocv_int(L, block[0], newsize, t);
            size[0] = newsize;  /* update only when everything else is OK */
            return newblock;
        }

        //-------------------------------

		public static object luaM_toobig(LuaState.lua_State L) 
		{
			LuaDebug.luaG_runerror(L, CLib.CharPtr.toCharPtr("memory allocation error: block too big"));
			return null;  /* to avoid warnings */
		}

		/*
		 ** generic allocation routine.
		 */
		public static object luaM_realloc_(LuaState.lua_State L, ClassType t)
		{
			int unmanaged_size = (int)CLib.GetUnmanagedSize(t);
			int nsize = unmanaged_size;
			object new_obj = t.Alloc();
			AddTotalBytes(L, nsize);
			return new_obj;
		}

		public static object luaM_realloc__Proto(LuaState.lua_State L, ClassType t)
		{
			int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
			int nsize = unmanaged_size;
            LuaObject.Proto new_obj = (LuaObject.Proto)t.Alloc();//System.Activator.CreateInstance(typeof(T));
			AddTotalBytes(L, nsize);
			return new_obj;
		}

        public static object luaM_realloc__Closure(LuaState.lua_State L, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int nsize = unmanaged_size;
            LuaObject.Closure new_obj = (LuaObject.Closure)t.Alloc();//System.Activator.CreateInstance(typeof(T));
            AddTotalBytes(L, nsize);
            return new_obj;
        }

        public static object luaM_realloc__UpVal(LuaState.lua_State L, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int nsize = unmanaged_size;
            LuaObject.UpVal new_obj = (LuaObject.UpVal)t.Alloc();//System.Activator.CreateInstance(typeof(T));
            AddTotalBytes(L, nsize);
            return new_obj;
        }

        public static object luaM_realloc__lua_State(LuaState.lua_State L, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int nsize = unmanaged_size;
            LuaState.lua_State new_obj = (LuaState.lua_State)t.Alloc();//System.Activator.CreateInstance(typeof(T));
            AddTotalBytes(L, nsize);
            return new_obj;
        }

        public static object luaM_realloc__Table(LuaState.lua_State L, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int nsize = unmanaged_size;
            LuaObject.Table new_obj = (LuaObject.Table)t.Alloc();//System.Activator.CreateInstance(typeof(T));
            AddTotalBytes(L, nsize);
            return new_obj;
        }







        //---------------------------------


		//public static object luaM_realloc_<T>(lua_State L, T obj, ClassType t)
		//{
		//	int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T))
		//	int old_size = (obj == null) ? 0 : unmanaged_size;
		//	int osize = old_size * unmanaged_size;
		//	int nsize = unmanaged_size;
        //   T new_obj = (T)t.Alloc(); //System.Activator.CreateInstance(typeof(T))
		//	SubtractTotalBytes(L, osize);
		//	AddTotalBytes(L, nsize);
		//	return new_obj;
        //}

		//public static object luaM_realloc_<T>(lua_State L, T[] old_block, int new_size, ClassType t)
		//{
		//	int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
		//	int old_size = (old_block == null) ? 0 : old_block.Length;
		//	int osize = old_size * unmanaged_size;
		//	int nsize = new_size * unmanaged_size;
		//	T[] new_block = new T[new_size];
		//	for (int i = 0; i < Math.Min(old_size, new_size); i++)
		//	{
		//		new_block[i] = old_block[i];
		//	}
		//	for (int i = old_size; i < new_size; i++)
		//	{
        //       new_block[i] = (T)t.Alloc();// System.Activator.CreateInstance(typeof(T));
		//	}
		//	if (CanIndex(t))
		//	{
        //		for (int i = 0; i < new_size; i++)
		//		{
		//			ArrayElement elem = new_block[i] as ArrayElement;
		//			ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
		//			elem.set_index(i);
		//			elem.set_array(new_block);
		//		}
		//	}
		//	SubtractTotalBytes(L, osize);
		//	AddTotalBytes(L, nsize);
		//	return new_block;
		//}

        public static object luaM_realloc__Table(LuaState.lua_State L, LuaObject.Table[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            LuaObject.Table[] new_block = new LuaObject.Table[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (LuaObject.Table)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    LuaObject.ArrayElement elem = new_block[i] as LuaObject.ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__UpVal(LuaState.lua_State L, LuaObject.UpVal[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            LuaObject.UpVal[] new_block = new LuaObject.UpVal[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (LuaObject.UpVal)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    LuaObject.ArrayElement elem = new_block[i] as LuaObject.ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__char(LuaState.lua_State L, char[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            char[] new_block = new char[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (char)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t)) // FIXME:not necessary
            {
                /*
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
                */
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__TValue(LuaState.lua_State L, LuaObject.TValue[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            LuaObject.TValue[] new_block = new LuaObject.TValue[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (LuaObject.TValue)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    LuaObject.ArrayElement elem = new_block[i] as LuaObject.ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__TString(LuaState.lua_State L, LuaObject.TString[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            LuaObject.TString[] new_block = new LuaObject.TString[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (LuaObject.TString)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    LuaObject.ArrayElement elem = new_block[i] as LuaObject.ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__Udata(LuaState.lua_State L, LuaObject.Udata[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            LuaObject.Udata[] new_block = new LuaObject.Udata[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (LuaObject.Udata)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    LuaObject.ArrayElement elem = new_block[i] as LuaObject.ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__CallInfo(LuaState.lua_State L, LuaState.CallInfo[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            LuaState.CallInfo[] new_block = new LuaState.CallInfo[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (LuaState.CallInfo)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    LuaObject.ArrayElement elem = new_block[i] as LuaObject.ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__long(LuaState.lua_State L, long[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            long[] new_block = new long[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (long)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t)) //FIXME: not necessary
            {
                /*
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
                */
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__int(LuaState.lua_State L, int[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            int[] new_block = new int[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (int)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t)) //FIXME: not necessary
            {
                /*
                for (int i = 0; i < new_size; i++)
                {
                    ArrayElement elem = new_block[i] as ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
                */
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__Proto(LuaState.lua_State L, LuaObject.Proto[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            LuaObject.Proto[] new_block = new LuaObject.Proto[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (LuaObject.Proto)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    LuaObject.ArrayElement elem = new_block[i] as LuaObject.ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__LocVar(LuaState.lua_State L, LuaObject.LocVar[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            LuaObject.LocVar[] new_block = new LuaObject.LocVar[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (LuaObject.LocVar)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    LuaObject.ArrayElement elem = new_block[i] as LuaObject.ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__Node(LuaState.lua_State L, LuaObject.Node[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            LuaObject.Node[] new_block = new LuaObject.Node[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (LuaObject.Node)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    LuaObject.ArrayElement elem = new_block[i] as LuaObject.ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }

        public static object luaM_realloc__GCObject(LuaState.lua_State L, LuaState.GCObject[] old_block, int new_size, ClassType t)
        {
            int unmanaged_size = (int)t.GetUnmanagedSize();//CLib.GetUnmanagedSize(typeof(T));
            int old_size = (old_block == null) ? 0 : old_block.Length;
            int osize = old_size * unmanaged_size;
            int nsize = new_size * unmanaged_size;
            LuaState.GCObject[] new_block = new LuaState.GCObject[new_size];
            for (int i = 0; i < Math.Min(old_size, new_size); i++)
            {
                new_block[i] = old_block[i];
            }
            for (int i = old_size; i < new_size; i++)
            {
                new_block[i] = (LuaState.GCObject)t.Alloc();// System.Activator.CreateInstance(typeof(T));
            }
            if (CanIndex(t))
            {
                for (int i = 0; i < new_size; i++)
                {
                    LuaObject.ArrayElement elem = new_block[i] as LuaObject.ArrayElement;
                    ClassType.Assert(elem != null, String.Format("Need to derive type {0} from ArrayElement", t.GetTypeString()));
                    elem.set_index(i);
                    elem.set_array(new_block);
                }
            }
            SubtractTotalBytes(L, osize);
            AddTotalBytes(L, nsize);
            return new_block;
        }


		public static bool CanIndex(ClassType t)
		{
            return t.CanIndex();
		}

		public static void AddTotalBytes(LuaState.lua_State L, int num_bytes) 
		{
			LuaState.G(L).totalbytes += (int/*uint*/)num_bytes; 
		}
		
		public static void SubtractTotalBytes(LuaState.lua_State L, int num_bytes) 
		{ 
			LuaState.G(L).totalbytes -= (int/*uint*/)num_bytes; 
		}

		//static void AddTotalBytes(lua_State L, uint num_bytes) {G(L).totalbytes += num_bytes;}
		//static void SubtractTotalBytes(lua_State L, uint num_bytes) {G(L).totalbytes -= num_bytes;}
	}
}
