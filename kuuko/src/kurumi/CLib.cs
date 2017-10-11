/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2017-10-01
 * Time: 13:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace kurumi
{
	public class CLib
	{
		// misc stuff needed for the compile

		public static bool isalpha(char c) 
		{ 
			return Char.IsLetter(c); 
		}
		
		public static bool iscntrl(char c) 
		{ 
			return Char.IsControl(c); 
		}
		
		public static bool isdigit(char c) 
		{ 
			return Char.IsDigit(c); 
		}
		
		public static bool islower(char c) 
		{ 
			return Char.IsLower(c); 
		}
		
		public static bool ispunct(char c) 
		{ 
			return ClassType.IsPunctuation(c); 
		}
		
		public static bool isspace(char c) 
		{ 
			return (c==' ') || (c>=(char)0x09 && c<=(char)0x0D); 
		}
		
		public static bool isupper(char c) 
		{ 
			return Char.IsUpper(c); 
		}
		
		public static bool isalnum(char c) 
		{ 
			return Char.IsLetterOrDigit(c); 
		}
		
		public static bool isxdigit(char c) 
		{ 
			return "0123456789ABCDEFabcdef".IndexOf(c) >= 0; 
		}

		public static bool isalpha(int c) 
		{ 
			return Char.IsLetter((char)c); 
		}
		
		public static bool iscntrl(int c) 
		{ 
			return Char.IsControl((char)c); 
		}
		
		public static bool isdigit(int c) 
		{ 
			return Char.IsDigit((char)c); 
		}
		
		public static bool islower(int c) 
		{ 
			return Char.IsLower((char)c); 
		}
		
		public static bool ispunct(int c) 
		{ 
			return ((char)c != ' ') && !isalnum((char)c); 
		} // *not* the same as Char.IsPunctuation
		
		public static bool isspace(int c) 
		{ 
			return ((char)c == ' ') || ((char)c >= (char)0x09 && (char)c <= (char)0x0D); 
		}
		
		public static bool isupper(int c) 
		{ 
			return Char.IsUpper((char)c); 
		}
		
		public static bool isalnum(int c) 
		{ 
			return Char.IsLetterOrDigit((char)c); 
		}

		public static char tolower(char c) 
		{ 
			return Char.ToLower(c); 
		}
		
		public static char toupper(char c)
		{ 
			return Char.ToUpper(c); 
		}
		
		public static char tolower(int c)
		{ 
			return Char.ToLower((char)c); 
		}
		
		public static char toupper(int c) 
		{ 
			return Char.ToUpper((char)c); 
		}

		public static long/*ulong*/ strtoul(CharPtr s, /*out*/ CharPtr[] end, int base_)
		{
			try
			{
				end[0] = new CharPtr(s.chars, s.index);

				// skip over any leading whitespace
				while (end[0].get(0) == ' ')
				{
					end[0] = end[0].next();
				}

				// ignore any leading 0x
				if ((end[0].get(0) == '0') && (end[0].get(1) == 'x'))
				{
					end[0] = end[0].next().next();
				}
				else if ((end[0].get(0) == '0') && (end[0].get(1) == 'X'))
				{
					end[0] = end[0].next().next();
				}

				// do we have a leading + or - sign?
				bool negate = false;
				if (end[0].get(0) == '+')
				{
					end[0] = end[0].next();
				}
				else if (end[0].get(0) == '-')
				{
					negate = true;
					end[0] = end[0].next();
				}

				// loop through all chars
				bool invalid = false;
				bool had_digits = false;
				long/*ulong*/ result = 0;
				while (true)
				{
					// get this char
					char ch = end[0].get(0);

					// which digit is this?
					int this_digit = 0;
					if (isdigit(ch))
					{
						this_digit = ch - '0';
					}
					else if (isalpha(ch))
					{
						this_digit = tolower(ch) - 'a' + 10;
					}
					else
					{
						break;
					}

					// is this digit valid?
					if (this_digit >= base_)
					{
						invalid = true;
					}
					else
					{
						had_digits = true;
						result = result * (long/*ulong*/)base_ + (long/*ulong*/)this_digit;
					}

					end[0] = end[0].next();
				}

				// were any of the digits invalid?
				if (invalid || (!had_digits))
				{
					end[0] = s;
					return /*UInt64.MaxValue*//*Int64.MaxValue*/long.MaxValue;
				}

				// if the value was a negative then negate it here
				if (negate)
				{
					result = (long/*ulong*/)-(long)result;
				}

				// ok, we're done
				return (long/*ulong*/)result;
			}
			catch
			{
				end[0] = s;
				return 0;
			}
		}

		public static void putchar(char ch)
		{
            StreamProxy.Write("" + ch);
		}

		public static void putchar(int ch)
		{
            StreamProxy.Write("" + (char)ch);
		}

		public static bool isprint(byte c)
		{
			return (c >= (byte)' ') && (c <= (byte)127);
		}

		public static int parse_scanf(string str, CharPtr fmt, params object[] argp)
		{
			int parm_index = 0;
			int index = 0;
			while (fmt.get(index) != 0)
			{
				if (fmt.get(index++) == '%')
				{
					switch (fmt.get(index++))
					{
						case 's':
							{
								argp[parm_index++] = str;
								break;
							}
						case 'c':
							{
								argp[parm_index++] = ClassType.ConvertToChar(str);
								break;
							}
						case 'd':
							{
                                argp[parm_index++] = ClassType.ConvertToInt32(str);
								break;
							}
						case 'l':
							{
                                argp[parm_index++] = ClassType.ConvertToDouble(str, null);
								break;
							}
						case 'f':
							{
                                argp[parm_index++] = ClassType.ConvertToDouble(str, null);
								break;
							}
						//case 'p':
						//    {
						//        result += "(pointer)";
						//        break;
						//    }
					}
				}
			}
			return parm_index;
		}

		public static void printf(CharPtr str, params object[] argv)
		{
			Tools.printf(str.ToString(), argv);
		}

		public static void sprintf(CharPtr buffer, CharPtr str, params object[] argv)
		{
			string temp = Tools.sprintf(str.ToString(), argv);
			strcpy(buffer, CharPtr.toCharPtr(temp));
		}

		public static int fprintf(StreamProxy stream, CharPtr str, params object[] argv)
		{
			string result = Tools.sprintf(str.ToString(), argv);
			char[] chars = result.ToCharArray();
			byte[] bytes = new byte[chars.Length];
			for (int i=0; i<chars.Length; i++)
			{
				bytes[i] = (byte)chars[i];
			}
			stream.Write(bytes, 0, bytes.Length);
			return 1;
		}

		public const int EXIT_SUCCESS = 0;
		public const int EXIT_FAILURE = 1;

		public static int errno()
		{
			return -1;	// todo: fix this - mjf
		}

		public static CharPtr strerror(int error)
		{
			return CharPtr.toCharPtr(String.Format("error #{0}", error)); // todo: check how this works - mjf
            //FIXME:
		}

		public static CharPtr getenv(CharPtr envname)
		{
			// todo: fix this - mjf
			//if (envname.Equals("LUA_PATH"))
			//return "MyPath";
			string result = Environment.GetEnvironmentVariable(envname.ToString());
			return result != null ? new CharPtr(result) : null;
		}

		public class CharPtr
		{
			public char[] chars;
			public int index;
	
	        //public char this[int offset] get
	        public char get(int offset)
	        {
	            return chars[index + offset];
	        }
	
	        //public char this[int offset] set
	        public void set(int offset, char val)
	        {
	            chars[index + offset] = val;
	        }
	
	        //public char this[long offset] get
	        public char get(long offset)
	        {
	            return chars[index + (int)offset];
	        }
	
	        //public char this[long offset] set
	        public void set(long offset, char val)
	        {
	            chars[index + (int)offset] = val;
	        }
			
	        //implicit operator CharPtr
	        public static CharPtr toCharPtr(string str) 
			{
				return new CharPtr(str); 
			}
	
			//implicit operator CharPtr
			public static CharPtr toCharPtr(char[] chars) 
			{ 
				return new CharPtr(chars); 
			}
			
			public CharPtr()
			{
				this.chars = null;
				this.index = 0;
			}
			
			public CharPtr(string str)
			{
				this.chars = (str + '\0').ToCharArray();
				this.index = 0;
			}
			
			public CharPtr(CharPtr ptr)
			{
				this.chars = ptr.chars;
				this.index = ptr.index;
			}
			
			public CharPtr(CharPtr ptr, int index)
			{
				this.chars = ptr.chars;
				this.index = index;
			}
			
			public CharPtr(char[] chars)
			{
				this.chars = chars;
				this.index = 0;
			}
			
			public CharPtr(char[] chars, int index)
			{
				this.chars = chars;
				this.index = index;
			}
			
			//public CharPtr(IntPtr ptr)
			//{
			//	this.chars = new char[0];
			//	this.index = 0;
			//}
			
			public static CharPtr plus(CharPtr ptr, int offset) 
			{ 
				return new CharPtr(ptr.chars, ptr.index + offset); 
			}
			
			public static CharPtr minus(CharPtr ptr, int offset) 
			{
				return new CharPtr(ptr.chars, ptr.index - offset); 
			}
			
			public void inc() 
			{
				this.index++; 
			}
			
			public void dec() 
			{ 
				this.index--; 
			}
			
			public CharPtr next() 
			{ 
				return new CharPtr(this.chars, this.index + 1); 
			}
			
			public CharPtr prev() 
			{ 
				return new CharPtr(this.chars, this.index - 1); 
			}
			
			public CharPtr add(int ofs) 
			{ 
				return new CharPtr(this.chars, this.index + ofs); 
			}
			
			public CharPtr sub(int ofs) 
			{ 
				return new CharPtr(this.chars, this.index - ofs); 
			}
	
	        //operator ==
	        public static bool isEqualChar(CharPtr ptr, char ch)
	        {
	            return ptr.get(0) == ch;
	        }
	
	        //operator ==
	        public static bool isEqualChar(char ch, CharPtr ptr) 
			{ 
				return ptr.get(0) == ch; 
			}
			
	        //operator !=
			public static bool isNotEqualChar(CharPtr ptr, char ch) 
			{ 
				return ptr.get(0) != ch; 
			}
			
	        //operator !=
	        public static bool isNotEqualChar(char ch, CharPtr ptr) 
			{ 
				return ptr.get(0) != ch; 
			}
			
			public static CharPtr plus(CharPtr ptr1, CharPtr ptr2)
			{
				string result = "";
				for (int i = 0; ptr1.get(i) != '\0'; i++)
				{
					result += ptr1.get(i);
				}
				for (int i = 0; ptr2.get(i) != '\0'; i++)
				{
					result += ptr2.get(i);
				}
				return new CharPtr(result);
			}
			
			public static int minus(CharPtr ptr1, CharPtr ptr2)
			{
				ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index - ptr2.index;
			}
			
	        //operator <
			public static bool lessThan(CharPtr ptr1, CharPtr ptr2)
			{
				ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index < ptr2.index;
			}
	        //operator <=
			public static bool lessEqual(CharPtr ptr1, CharPtr ptr2)
			{
				ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index <= ptr2.index;
			}
	        public static bool greaterThan(CharPtr ptr1, CharPtr ptr2)
			{
				ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index > ptr2.index;
			}
	        //operator >=
			public static bool greaterEqual(CharPtr ptr1, CharPtr ptr2)
			{
				ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index >= ptr2.index;
			}
	
	        //operator ==
	        public static bool isEqual(CharPtr ptr1, CharPtr ptr2)
	        {
	            object o1 = ptr1 as CharPtr;
	            object o2 = ptr2 as CharPtr;
	            if ((o1 == null) && (o2 == null))
	            {
	                return true;
	            }
	            if (o1 == null)
	            {
	                return false;
	            }
	            if (o2 == null)
	            {
	                return false;
	            }
	            return (ptr1.chars == ptr2.chars) && (ptr1.index == ptr2.index);
	        }
	
	        //operator !=
	        public static bool isNotEqual(CharPtr ptr1, CharPtr ptr2)
	        {
	            return !(CharPtr.isEqual(ptr1, ptr2));
	        }
			
			public override bool Equals(object o)
			{
				return CharPtr.isEqual(this, (o as CharPtr));
			}
			
			public override int GetHashCode()
			{
				return 0;
			}
			
			public override string ToString()
			{
				string result = "";
				for (int i = index; (i < chars.Length) && (chars[i] != '\0'); i++)
				{
					result += chars[i];
				}
				return result;
			}
		}
		
		
		


		//public static int memcmp(CharPtr ptr1, CharPtr ptr2, uint size) 
		//{ 
		//	return memcmp(ptr1, ptr2, (int)size); 
		//}
		
		public static int memcmp(CharPtr ptr1, CharPtr ptr2, int size)
		{
			for (int i = 0; i < size; i++)
			{
				if (ptr1.get(i) != ptr2.get(i))
				{
					if (ptr1.get(i) < ptr2.get(i))
					{
						return -1;
					}
					else
					{
						return 1;
					}
				}
			}
			return 0;
		}

		public static CharPtr memchr(CharPtr ptr, char c, int/*uint*/ count)
		{
			for (/*uint*/int i = 0; i < count; i++)
			{	
				if (ptr.get(i) == c)
				{
					return new CharPtr(ptr.chars, (int)(ptr.index + i));
				}
			}
			return null;
		}

		public static CharPtr strpbrk(CharPtr str, CharPtr charset)
		{
			for (int i = 0; str.get(i) != '\0'; i++) 
			{
				for (int j = 0; charset.get(j) != '\0'; j++)
				{
					if (str.get(i) == charset.get(j))
					{
						return new CharPtr(str.chars, str.index + i);
					}
				}
			}
			return null;
		}

		// find c in str
		public static CharPtr strchr(CharPtr str, char c)
		{
			for (int index = str.index; str.chars[index] != 0; index++)
			{	
				if (str.chars[index] == c)
				{
					return new CharPtr(str.chars, index);
				}
			}
			return null;
		}

		public static CharPtr strcpy(CharPtr dst, CharPtr src)
		{
			int i;
			for (i = 0; src.get(i) != '\0'; i++)
			{
				dst.set(i, src.get(i));
			}
			dst.set(i, '\0');
			return dst;
		}

		public static CharPtr strcat(CharPtr dst, CharPtr src)
		{
			int dst_index = 0;
			while (dst.get(dst_index) != '\0')
			{
				dst_index++;
			}
			int src_index = 0;
			while (src.get(src_index) != '\0')
			{
				dst.set(dst_index++, src.get(src_index++));
			}
			dst.set(dst_index++, '\0');
			return dst;
		}

		public static CharPtr strncat(CharPtr dst, CharPtr src, int count)
		{
			int dst_index = 0;
			while (dst.get(dst_index) != '\0')
			{
				dst_index++;
			}
			int src_index = 0;
			while ((src.get(src_index) != '\0') && (count-- > 0))
			{
				dst.set(dst_index++, src.get(src_index++));
			}
			return dst;
		}

		public static int/*uint*/ strcspn(CharPtr str, CharPtr charset)
		{
			//int index = str.ToString().IndexOfAny(charset.ToString().ToCharArray());
            int index = ClassType.IndexOfAny(str.ToString(), charset.ToString().ToCharArray());
            if (index < 0)
			{
				index = str.ToString().Length;
			}
			return /*(uint)*/index;
		}

		public static CharPtr strncpy(CharPtr dst, CharPtr src, int length)
		{
			int index = 0;
			while ((src.get(index) != '\0') && (index < length))
			{
				dst.set(index, src.get(index));
				index++;
			}
			while (index < length)
			{
				dst.set(index++, '\0');
			}
			return dst;
		}

		public static int strlen(CharPtr str)
		{
			int index = 0;
			while (str.get(index) != '\0')
			{
				index++;
			}
			return index;
		}

		public static Double/*lua_Number*/ fmod(Double/*lua_Number*/ a, Double/*lua_Number*/ b)
		{
			float quotient = (int)Math.Floor(a / b);
			return a - quotient * b;
		}

		public static Double/*lua_Number*/ modf(Double/*lua_Number*/ a, /*out*/ Double[]/*lua_Number*/ b)
		{
			b[0] = Math.Floor(a);
			return a - Math.Floor(a);
		}

		public static long lmod(Double/*lua_Number*/ a, Double/*lua_Number*/ b)
		{
			return (long)a % (long)b;
		}

		public static int getc(StreamProxy f)
		{
			return f.ReadByte();
		}

		public static void ungetc(int c, StreamProxy f)
		{
            f.ungetc(c);
		}

        public static StreamProxy stdout = StreamProxy.OpenStandardOutput();
        public static StreamProxy stdin = StreamProxy.OpenStandardInput();
        public static StreamProxy stderr = StreamProxy.OpenStandardError();
		public static int EOF = -1;

		public static void fputs(CharPtr str, StreamProxy stream)
		{
            StreamProxy.Write(str.ToString()); //FIXME:
		}

		public static int feof(StreamProxy s)
		{
			return (s.isEof()) ? 1 : 0;
		}

		public static int fread(CharPtr ptr, int size, int num, StreamProxy stream)
		{
			int num_bytes = num * size;
			byte[] bytes = new byte[num_bytes];
			try
			{
				int result = stream.Read(bytes, 0, num_bytes);
				for (int i = 0; i < result; i++)
				{
					ptr.set(i, (char)bytes[i]);
				}
				return result/size;
			}
			catch
			{
				return 0;
			}
		}

		public static int fwrite(CharPtr ptr, int size, int num, StreamProxy stream)
		{
			int num_bytes = num * size;
			byte[] bytes = new byte[num_bytes];
			for (int i = 0; i < num_bytes; i++)
			{
				bytes[i] = (byte)ptr.get(i);
			}
			try
			{
				stream.Write(bytes, 0, num_bytes);
			}
			catch
			{
				return 0;
			}
			return num;
		}

		public static int strcmp(CharPtr s1, CharPtr s2)
		{
			if (CharPtr.isEqual(s1, s2))
			{
				return 0;
			}
			if (CharPtr.isEqual(s1, null))
			{
				return -1;
			}
			if (CharPtr.isEqual(s2, null))
			{
				return 1;
			}

			for (int i = 0; ; i++)
			{
				if (s1.get(i) != s2.get(i))
				{
					if (s1.get(i) < s2.get(i))
					{
						return -1;
					}
					else
					{
						return 1;
					}
				}
				if (s1.get(i) == '\0')
				{
					return 0;
				}
			}
		}

		public static CharPtr fgets(CharPtr str, StreamProxy stream)
		{
			int index = 0;
			try
			{
				while (true)
				{
					str.set(index, (char)stream.ReadByte());
					if (str.get(index) == '\n')
					{
						break;
					}
					if (index >= str.chars.Length)
					{
						break;
					}
					index++;
				}
			}
			catch
			{
				
			}
			return str;
		}

		public static double frexp(double x, /*out*/ int[] expptr)
		{
			expptr[0] = ClassType.log2(x) + 1;
			double s = x / Math.Pow(2, expptr[0]);
			return s;
		}

		public static double ldexp(double x, int expptr)
		{
			return x * Math.Pow(2, expptr);
		}

		public static CharPtr strstr(CharPtr str, CharPtr substr)
		{
			int index = str.ToString().IndexOf(substr.ToString());
			if (index < 0)
			{
				return null;
			}
			return new CharPtr(CharPtr.plus(str, index));
		}

		public static CharPtr strrchr(CharPtr str, char ch)
		{
			int index = str.ToString().LastIndexOf(ch);
			if (index < 0)
			{
				return null;
			}
			return CharPtr.plus(str, index);
		}

		public static StreamProxy fopen(CharPtr filename, CharPtr mode)
		{
			string str = filename.ToString();
            string modeStr = "";
            for (int i = 0; mode.get(i) != '\0'; i++)
            {
                modeStr += mode.get(i);
            }
			try
			{
                StreamProxy result = new StreamProxy(str, modeStr);
                if (result.isOK)
                {
                    return result;
                }
                else
                {
                    return null;
                }
			}
			catch
			{
				return null;
			}
		}

		public static StreamProxy freopen(CharPtr filename, CharPtr mode, StreamProxy stream)
		{
			try
			{
				stream.Flush();
				stream.Close();
			}
			catch 
			{
					
			}
			return fopen(filename, mode);
		}

		public static void fflush(StreamProxy stream)
		{
			stream.Flush();
		}

		public static int ferror(StreamProxy stream)
		{
            //FIXME:
			return 0;	// todo: fix this - mjf
		}

		public static int fclose(StreamProxy stream)
		{
			stream.Close();
			return 0;
		}

		public static StreamProxy tmpfile()
		{
            //new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite);
            return StreamProxy.tmpfile();
		}

		public static int fscanf(StreamProxy f, CharPtr format, params object[] argp)
		{
			string str = StreamProxy.ReadLine(); //FIXME: f
			return parse_scanf(str, format, argp);
		}
		
		public static int fseek(StreamProxy f, long offset, int origin)
		{
            return f.Seek(offset, origin);
		}


		public static int ftell(StreamProxy f)
		{
			return (int)f.getPosition();
		}

		public static int clearerr(StreamProxy f)
		{
			//ClassType.Assert(false, "clearerr not implemented yet - mjf");
			return 0;
		}

		public static int setvbuf(StreamProxy stream, CharPtr buffer, int mode, int/*uint*/ size)
		{
            //FIXME:stream
			ClassType.Assert(false, "setvbuf not implemented yet - mjf");
			return 0;
		}

		//public static void memcpy<T>(T[] dst, T[] src, int length)
		//{
		//	for (int i = 0; i < length; i++)
		//	{
		//		dst[i] = src[i];
		//	}
        //}

		public static void memcpy_char(char[] dst, int offset, char[] src, int length)
		{
			for (int i=0; i<length; i++)
			{
				dst[offset+i] = src[i];
			}
		}

		public static void memcpy_char(char[] dst, char[] src, int srcofs, int length)
		{
			for (int i = 0; i < length; i++)
			{
				dst[i] = src[srcofs+i];
			}
		}

		//public static void memcpy(CharPtr ptr1, CharPtr ptr2, uint size) 
		//{ 
		//	memcpy(ptr1, ptr2, (int)size); 
		//}
		
		public static void memcpy(CharPtr ptr1, CharPtr ptr2, int size)
		{
			for (int i = 0; i < size; i++)
			{
				ptr1.set(i, ptr2.get(i));
			}
		}

		public static object VOID(object f) 
		{ 
			return f; 
		}

		public const double HUGE_VAL = /*System.*/Double.MaxValue;
		public const int/*uint*/ SHRT_MAX = /*System.UInt16*/short.MaxValue;

		public const int _IONBF = 0;
		public const int _IOFBF = 1;
		public const int _IOLBF = 2;

		public const int SEEK_SET = 0;
		public const int SEEK_CUR = 1;
		public const int SEEK_END = 2;

		// one of the primary objectives of this port is to match the C version of Lua as closely as
		// possible. a key part of this is also matching the behaviour of the garbage collector, as
		// that affects the operation of things such as weak tables. in order for this to occur the
		// size of structures that are allocated must be reported as identical to their C++ equivelents.
		// that this means that variables such as global_State.totalbytes no longer indicate the true
		// amount of memory allocated.
		public static int GetUnmanagedSize(ClassType t)
		{
            return t.GetUnmanagedSize();
		}
		
		
		
		
		
		
		
		//----------------------------------------		
		//https://github.com/NLua/KopiLua/blob/master/KopiLua/src/loslib.cs

		//
		// This strftime implementation has been made following the
		// Sanos OS open-source strftime.c implementation at
		// http://www.jbox.dk/sanos/source/lib/strftime.c.html
		
		public static uint strftime(CharPtr s, uint maxsize, CharPtr format, DateTimeProxy t)
		{
			int sIndex = s.index;

			CharPtr p = StrFTimeFmt((format as object) == null ? CharPtr.toCharPtr("%c") : format, t.stm, s, s.add((int)maxsize));
			if (CLib.CharPtr.isEqual(p, CLib.CharPtr.plus(s, (int)maxsize))) return 0;
			p.set(0, '\0');

			return (uint)Math.Abs(s.index - sIndex);
		}

		private static CharPtr StrFTimeFmt(CharPtr baseFormat, DateTime t, CharPtr pt, CharPtr ptlim)
		{
			CharPtr format = new CharPtr(baseFormat);

			for (; format.get(0) != 0; format.inc())
			{

				if (format.get(0) == '%')
				{

					format.inc();

					if (format.get(0) == 'E')
					{
						format.inc(); // Alternate Era is ignored
					}
					else if (format.get(0) == 'O')
					{
						format.inc(); // Alternate numeric symbols is ignored
					}

					switch (format.get(0))
					{
						case '\0':
							format.dec();
							break;

						case 'A': // Full day of week
							//pt = _add((t->tm_wday < 0 || t->tm_wday > 6) ? "?" : _days[t->tm_wday], pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("dddd")), pt, ptlim);
							continue;

						case 'a': // Abbreviated day of week
							//pt = _add((t->tm_wday < 0 || t->tm_wday > 6) ? "?" : _days_abbrev[t->tm_wday], pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("ddd")), pt, ptlim);
							continue;

						case 'B': // Full month name
							//pt = _add((t->tm_mon < 0 || t->tm_mon > 11) ? "?" : _months[t->tm_mon], pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("MMMM")), pt, ptlim);
							continue;

						case 'b': // Abbreviated month name
						case 'h': // Abbreviated month name
							//pt = _add((t->tm_mon < 0 || t->tm_mon > 11) ? "?" : _months_abbrev[t->tm_mon], pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("MMM")), pt, ptlim);
							continue;

						case 'C': // First two digits of year (a.k.a. Year divided by 100 and truncated to integer (00-99))
							//pt = _conv((t->tm_year + TM_YEAR_BASE) / 100, "%02d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("yyyy").Substring(0, 2)), pt, ptlim);
							continue;

						case 'c': // Abbreviated date/time representation (e.g. Thu Aug 23 14:55:02 2001)
							//pt = StrFTimeFmt(CLib.CharPtr.toCharPtr("%a %b %e %H:%M:%S %Y"), t, pt, ptlim); //FIXME:
							pt = StrFTimeFmt(CLib.CharPtr.toCharPtr("%m/%d/%y %H:%M:%S"), t, pt, ptlim); //FIXME:???
							continue;

						case 'D': // Short MM/DD/YY date
							pt = StrFTimeFmt(CLib.CharPtr.toCharPtr("%m/%d/%y"), t, pt, ptlim);
							continue;

						case 'd': // Day of the month, zero-padded (01-31)
							//pt = _conv(t->tm_mday, "%02d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("dd")), pt, ptlim);
							continue;

						case 'e': // Day of the month, space-padded ( 1-31)
							//pt = _conv(t->tm_mday, "%2d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.Day.ToString().PadLeft(2, ' ')), pt, ptlim);
							continue;

						case 'F': // Short YYYY-MM-DD date
							pt = StrFTimeFmt(CLib.CharPtr.toCharPtr("%Y-%m-%d"), t, pt, ptlim);
							continue;

						case 'H': // Hour in 24h format (00-23)
							//pt = _conv(t->tm_hour, "%02d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("HH")), pt, ptlim);
							continue;

						case 'I': // Hour in 12h format (01-12)
							//pt = _conv((t->tm_hour % 12) ? (t->tm_hour % 12) : 12, "%02d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("hh")), pt, ptlim);
							continue;

						case 'j': // Day of the year (001-366)
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.DayOfYear.ToString().PadLeft(3, ' ')), pt, ptlim);
							continue;

						case 'k': // (Non-standard) // Hours in 24h format, space-padded ( 1-23)
							//pt = _conv(t->tm_hour, "%2d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("%H").PadLeft(2, ' ')), pt, ptlim);
							continue;

						case 'l': // (Non-standard) // Hours in 12h format, space-padded ( 1-12)
							//pt = _conv((t->tm_hour % 12) ? (t->tm_hour % 12) : 12, "%2d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("%h").PadLeft(2, ' ')), pt, ptlim);
							continue;

						case 'M': // Minute (00-59)
							//pt = _conv(t->tm_min, "%02d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("mm")), pt, ptlim);
							continue;

						case 'm': // Month as a decimal number (01-12)
							//pt = _conv(t->tm_mon + 1, "%02d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("MM")), pt, ptlim);
							continue;

						case 'n': // New-line character.
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(Environment.NewLine), pt, ptlim);
							continue;

						case 'p': // AM or PM designation (locale dependent).
							//pt = _add((t->tm_hour >= 12) ? "pm" : "am", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("tt")), pt, ptlim);
							continue;

						case 'R': // 24-hour HH:MM time, equivalent to %H:%M
							pt = StrFTimeFmt(CLib.CharPtr.toCharPtr("%H:%M"), t, pt, ptlim);
							continue;

						case 'r': // 12-hour clock time (locale dependent).
							pt = StrFTimeFmt(CLib.CharPtr.toCharPtr("%I:%M:%S %p"), t, pt, ptlim);
							continue;

						case 'S': // Second ((00-59)
							//pt = _conv(t->tm_sec, "%02d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("ss")), pt, ptlim);
							continue;

						case 'T': // ISO 8601 time format (HH:MM:SS), equivalent to %H:%M:%S
							pt = StrFTimeFmt(CLib.CharPtr.toCharPtr("%H:%M:%S"), t, pt, ptlim);
							continue;

						case 't': // Horizontal-tab character
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr("\t"), pt, ptlim);
							continue;

						case 'U': // Week number with the first Sunday as the first day of week one (00-53)
							//pt = _conv((t->tm_yday + 7 - t->tm_wday) / 7, "%02d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(t, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday).ToString()), pt, ptlim);
							continue;

						case 'u': // ISO 8601 weekday as number with Monday as 1 (1-7) (locale independant).
							//pt = _conv((t->tm_wday == 0) ? 7 : t->tm_wday, "%d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.DayOfWeek == DayOfWeek.Sunday ? "7" : ((int)t.DayOfWeek).ToString()), pt, ptlim);
							continue;

						case 'G':   // ISO 8601 year (four digits)
						case 'g':  // ISO 8601 year (two digits)
						case 'V':   // ISO 8601 week number
							// See http://stackoverflow.com/questions/11154673/get-the-correct-week-number-of-a-given-date
							DateTime isoTime = t;
							DayOfWeek day = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(isoTime);
							if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
							{
								isoTime = isoTime.AddDays(3);
							}

							if (format.get(0) == 'V') // ISO 8601 week number
							{
								int isoWeek = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(isoTime, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
								pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(isoWeek.ToString()), pt, ptlim);
							}
							else
							{
								string isoYear = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetYear(isoTime).ToString(); // ISO 8601 year (four digits)

								if (format.get(0) == 'g') // ISO 8601 year (two digits)
								{
									isoYear = isoYear.Substring(isoYear.Length - 2, 2);
								}
								pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(isoYear), pt, ptlim);
							}

							continue;

						case 'W': // Week number with the first Monday as the first day of week one (00-53)
							//pt = _conv((t->tm_yday + 7 - (t->tm_wday ? (t->tm_wday - 1) : 6)) / 7, "%02d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(t, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday).ToString()), pt, ptlim);
							continue;

						case 'w': // Weekday as a decimal number with Sunday as 0 (0-6)
							//pt = _conv(t->tm_wday, "%d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(((int)t.DayOfWeek).ToString()), pt, ptlim);
							continue;

						case 'X': // Long time representation (locale dependent)
							//pt = _fmt("%H:%M:%S", t, pt, ptlim); // fails to comply with spec!
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("%T")), pt, ptlim);
							continue;

						case 'x': // Short date representation (locale dependent)
							//pt = _fmt("%m/%d/%y", t, pt, ptlim); // fails to comply with spec!
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("%d")), pt, ptlim);
							continue;

						case 'y': // Last two digits of year (00-99)
							//pt = _conv((t->tm_year + TM_YEAR_BASE) % 100, "%02d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.ToString("yy")), pt, ptlim);
							continue;

						case 'Y': // Full year (all digits)
							//pt = _conv(t->tm_year + TM_YEAR_BASE, "%04d", pt, ptlim);
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(t.Year.ToString()), pt, ptlim);
							continue;

						case 'Z': // Timezone name or abbreviation (locale dependent) or nothing if unavailable (e.g. CDT)
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(TimeZone.CurrentTimeZone.StandardName), pt, ptlim);
							continue;

						case 'z': // ISO 8601 offset from UTC in timezone (+/-hhmm), or nothing if unavailable
							TimeSpan ts = TimeZone.CurrentTimeZone.GetUtcOffset(t);
							string offset = (ts.Ticks < 0 ? "-" : "+") + ts.TotalHours.ToString("#00") + ts.Minutes.ToString("00");
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr(offset), pt, ptlim);
							continue;

						case '%': // Add '%'
							pt = StrFTimeAdd(CLib.CharPtr.toCharPtr("%"), pt, ptlim);
							continue;

						default:
							break;
					}
				}

				if (pt == ptlim) break;

				pt.set(0, format.get(0));
				pt.inc();
			}

			return pt;
		}

		private static CharPtr StrFTimeAdd(CharPtr str, CharPtr pt, CharPtr ptlim)
		{
			pt.set(0, str.get(0));
			str = str.next();

			while (CLib.CharPtr.lessThan(pt, ptlim) && pt.get(0) != 0)
			{
				pt.inc();

				pt.set(0, str.get(0));
				str = str.next();
			}
			return pt;
		} 
		//-------------------
	}
}
