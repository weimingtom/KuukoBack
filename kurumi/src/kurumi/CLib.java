package kurumi;

public class CLib {

	// misc stuff needed for the compile

	public static boolean isalpha(char c) {
		return Character.isLetter(c);
	}

	public static boolean iscntrl(char c) {
		return Character.isISOControl(c);
	}

	public static boolean isdigit(char c) {
		return Character.isDigit(c);
	}

	public static boolean islower(char c) {
		return Character.isLowerCase(c);
	}

	public static boolean ispunct(char c) {
		return ClassType.IsPunctuation(c);
	}

	public static boolean isspace(char c) {
		return (c==' ') || (c>=(char)0x09 && c<=(char)0x0D);
	}

	public static boolean isupper(char c) {
		return Character.isUpperCase(c);
	}

	public static boolean isalnum(char c) {
		return Character.isLetterOrDigit(c);
	}

	public static boolean isxdigit(char c) {
		return (new String("0123456789ABCDEFabcdef")).indexOf(c) >= 0;
	}

	public static boolean isalpha(int c) {
		return Character.isLetter((char)c);
	}

	public static boolean iscntrl(int c) {
		return Character.isISOControl((char)c);
	}

	public static boolean isdigit(int c) {
		return Character.isDigit((char)c);
	}

	public static boolean islower(int c) {
		return Character.isLowerCase((char)c);
	}

	public static boolean ispunct(int c) {
		return ((char)c != ' ') && !isalnum((char)c);
	} // *not* the same as Char.IsPunctuation

	public static boolean isspace(int c) {
		return ((char)c == ' ') || ((char)c >= (char)0x09 && (char)c <= (char)0x0D);
	}

	public static boolean isupper(int c) {
		return Character.isUpperCase((char)c);
	}

	public static boolean isalnum(int c) {
		return Character.isLetterOrDigit((char)c);
	}

	public static char tolower(char c) {
		return Character.toLowerCase(c);
	}

	public static char toupper(char c) {
		return Character.toUpperCase(c);
	}

	public static char tolower(int c) {
		return Character.toLowerCase((char)c);
	}

	public static char toupper(int c) {
		return Character.toUpperCase((char)c);
	}

	public static long strtoul(CharPtr s, CharPtr[] end, int base_) { //out - ulong
		try {
			end[0] = new CharPtr(s.chars, s.index);

			// skip over any leading whitespace
			while (end[0].get(0) == ' ') {
				end[0] = end[0].next();
			}

			// ignore any leading 0x
			if ((end[0].get(0) == '0') && (end[0].get(1) == 'x')) {
				end[0] = end[0].next().next();
			}
			else if ((end[0].get(0) == '0') && (end[0].get(1) == 'X')) {
				end[0] = end[0].next().next();
			}

			// do we have a leading + or - sign?
			boolean negate = false;
			if (end[0].get(0) == '+') {
				end[0] = end[0].next();
			}
			else if (end[0].get(0) == '-') {
				negate = true;
				end[0] = end[0].next();
			}

			// loop through all chars
			boolean invalid = false;
			boolean had_digits = false;
			long result = 0; //ulong
			while (true) {
				// get this char
				char ch = end[0].get(0);

				// which digit is this?
				int this_digit = 0;
				if (isdigit(ch)) {
					this_digit = ch - '0';
				}
				else if (isalpha(ch)) {
					this_digit = tolower(ch) - 'a' + 10;
				}
				else {
					break;
				}

				// is this digit valid?
				if (this_digit >= base_) {
					invalid = true;
				}
				else {
					had_digits = true;
					result = result * (long)base_ + (long)this_digit; //ulong - ulong
				}

				end[0] = end[0].next();
			}

			// were any of the digits invalid?
			if (invalid || (!had_digits)) {
				end[0] = s;
				return Long.MAX_VALUE; //Int64.MaxValue - UInt64.MaxValue
			}

			// if the value was a negative then negate it here
			if (negate) {
				result = (long)-(long)result; //ulong
			}

			// ok, we're done
			return (long)result; //ulong
		}
		catch (java.lang.Exception e) {
			end[0] = s;
			return 0;
		}
	}

	public static void putchar(char ch) {
		StreamProxy.Write("" + ch);
	}

	public static void putchar(int ch) {
		StreamProxy.Write("" + (char)ch);
	}

	public static boolean isprint(byte c) {
		return (c >= (byte)' ') && (c <= (byte)127);
	}

	public static int parse_scanf(String str, CharPtr fmt, Object... argp) {
		int parm_index = 0;
		int index = 0;
		while (fmt.get(index) != 0) {
			if (fmt.get(index++) == '%') {
				switch (fmt.get(index++)) {
					case 's': {
							argp[parm_index++] = str;
							break;
						}
					case 'c': {
							argp[parm_index++] = ClassType.ConvertToChar(str);
							break;
						}
					case 'd': {
							argp[parm_index++] = ClassType.ConvertToInt32(str);
							break;
						}
					case 'l': {
							argp[parm_index++] = ClassType.ConvertToDouble(str, null);
							break;
						}
					case 'f': {
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

	public static void printf(CharPtr str, Object... argv) {
		Tools.printf(str.toString(), argv);
	}

	public static void sprintf(CharPtr buffer, CharPtr str, Object... argv) {
		String temp = Tools.sprintf(str.toString(), argv);
		strcpy(buffer, CharPtr.toCharPtr(temp));
	}

	public static int fprintf(StreamProxy stream, CharPtr str, Object... argv) {
		String result = Tools.sprintf(str.toString(), argv);
		char[] chars = result.toCharArray();
		byte[] bytes = new byte[chars.length];
		for (int i=0; i<chars.length; i++) {
			bytes[i] = (byte)chars[i];
		}
		stream.Write(bytes, 0, bytes.length);
		return 1;
	}

	public static final int EXIT_SUCCESS = 0;
	public static final int EXIT_FAILURE = 1;

	public static int errno() {
		return -1; // todo: fix this - mjf
	}

	public static CharPtr strerror(int error) {
		return CharPtr.toCharPtr(String.format("error #%1$s", error)); // todo: check how this works - mjf
		//FIXME:
	}

	public static CharPtr getenv(CharPtr envname) {
		// todo: fix this - mjf
		//if (envname.Equals("LUA_PATH"))
		//return "MyPath";
		return null;
	}

	public static class CharPtr {
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
		public static CharPtr toCharPtr(String str) 
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
		
		public CharPtr(String str) 
		{
			this.chars = (str + '\0').toCharArray();
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
		public static boolean isEqualChar(CharPtr ptr, char ch) 
        {
            return ptr.get(0) == ch;
        }

        //operator ==
		public static boolean isEqualChar(char ch, CharPtr ptr) 
		{ 
			return ptr.get(0) == ch; 
		}
		
        //operator !=
		public static boolean isNotEqualChar(CharPtr ptr, char ch) 
		{ 
			return ptr.get(0) != ch; 
		}
		
        //operator !=
		public static boolean isNotEqualChar(char ch, CharPtr ptr) 
		{ 
			return ptr.get(0) != ch; 
		}
		
		public static CharPtr plus(CharPtr ptr1, CharPtr ptr2)
		{
			String result = "";
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
		public static boolean lessThan(CharPtr ptr1, CharPtr ptr2) 
		{
			ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index < ptr2.index;
		}
        //operator <=
		public static boolean lessEqual(CharPtr ptr1, CharPtr ptr2) 
		{
			ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index <= ptr2.index;
		}
		public static boolean greaterThan(CharPtr ptr1, CharPtr ptr2) 
		{
			ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index > ptr2.index;
		}
        //operator >=
		public static boolean greaterEqual(CharPtr ptr1, CharPtr ptr2) 
		{
			ClassType.Assert(ptr1.chars == ptr2.chars); return ptr1.index >= ptr2.index;
		}

        //operator ==
		public static boolean isEqual(CharPtr ptr1, CharPtr ptr2) 
        {
			Object o1 = (CharPtr)((ptr1 instanceof CharPtr) ? ptr1 : null);
			Object o2 = (CharPtr)((ptr2 instanceof CharPtr) ? ptr2 : null);
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
		public static boolean isNotEqual(CharPtr ptr1, CharPtr ptr2) 
        {
            return !(CharPtr.isEqual(ptr1, ptr2));
        }
		
		@Override
		public boolean equals(Object o) 
		{
			return CharPtr.isEqual(this, ((CharPtr)((o instanceof CharPtr) ? o : null)));
		}
		
		@Override
		public int hashCode() 
		{
			return 0;
		}
		
		@Override
		public String toString() 
		{
			String result = "";
			for (int i = index; (i < chars.length) && (chars[i] != '\0'); i++) 
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

	public static int memcmp(CharPtr ptr1, CharPtr ptr2, int size) {
		for (int i = 0; i < size; i++) {
			if (ptr1.get(i) != ptr2.get(i)) {
				if (ptr1.get(i) < ptr2.get(i)) {
					return -1;
				}
				else {
					return 1;
				}
			}
		}
		return 0;
	}

	public static CharPtr memchr(CharPtr ptr, char c, int count) { //uint
		for (int i = 0; i < count; i++) { //uint
			if (ptr.get(i) == c) {
				return new CharPtr(ptr.chars, (int)(ptr.index + i));
			}
		}
		return null;
	}

	public static CharPtr strpbrk(CharPtr str, CharPtr charset) {
		for (int i = 0; str.get(i) != '\0'; i++) {
			for (int j = 0; charset.get(j) != '\0'; j++) {
				if (str.get(i) == charset.get(j)) {
					return new CharPtr(str.chars, str.index + i);
				}
			}
		}
		return null;
	}

	// find c in str
	public static CharPtr strchr(CharPtr str, char c) {
		for (int index = str.index; str.chars[index] != 0; index++) {
			if (str.chars[index] == c) {
				return new CharPtr(str.chars, index);
			}
		}
		return null;
	}

	public static CharPtr strcpy(CharPtr dst, CharPtr src) {
		int i;
		for (i = 0; src.get(i) != '\0'; i++) {
			dst.set(i, src.get(i));
		}
		dst.set(i, '\0');
		return dst;
	}

	public static CharPtr strcat(CharPtr dst, CharPtr src) {
		int dst_index = 0;
		while (dst.get(dst_index) != '\0') {
			dst_index++;
		}
		int src_index = 0;
		while (src.get(src_index) != '\0') {
			dst.set(dst_index++, src.get(src_index++));
		}
		dst.set(dst_index++, '\0');
		return dst;
	}

	public static CharPtr strncat(CharPtr dst, CharPtr src, int count) {
		int dst_index = 0;
		while (dst.get(dst_index) != '\0') {
			dst_index++;
		}
		int src_index = 0;
		while ((src.get(src_index) != '\0') && (count-- > 0)) {
			dst.set(dst_index++, src.get(src_index++));
		}
		return dst;
	}

	public static int strcspn(CharPtr str, CharPtr charset) { //uint
		//int index = str.ToString().IndexOfAny(charset.ToString().ToCharArray());
		int index = ClassType.IndexOfAny(str.toString(), charset.toString().toCharArray());
		if (index < 0) {
			index = str.toString().length();
		}
		return index; //(uint)
	}

	public static CharPtr strncpy(CharPtr dst, CharPtr src, int length) {
		int index = 0;
		while ((src.get(index) != '\0') && (index < length)) {
			dst.set(index, src.get(index));
			index++;
		}
		while (index < length) {
			dst.set(index++, '\0');
		}
		return dst;
	}

	public static int strlen(CharPtr str) {
		int index = 0;
		while (str.get(index) != '\0') {
			index++;
		}
		return index;
	}

	public static double fmod(double a, double b) { //lua_Number - lua_Number - lua_Number
		float quotient = (int)Math.floor(a / b);
		return a - quotient * b;
	}

	public static double modf(double a, double[] b) { //lua_Number - out - lua_Number - lua_Number
		b[0] = Math.floor(a);
		return a - Math.floor(a);
	}

	public static long lmod(double a, double b) { //lua_Number - lua_Number
		return (long)a % (long)b;
	}

	public static int getc(StreamProxy f) {
		return f.ReadByte();
	}

	public static void ungetc(int c, StreamProxy f) {
		f.ungetc(c);
	}

	public static StreamProxy stdout = StreamProxy.OpenStandardOutput();
	public static StreamProxy stdin = StreamProxy.OpenStandardInput();
	public static StreamProxy stderr = StreamProxy.OpenStandardError();
	public static int EOF = -1;

	public static void fputs(CharPtr str, StreamProxy stream) {
		StreamProxy.Write(str.toString()); //FIXME:
	}

	public static int feof(StreamProxy s) {
		return (s.isEof()) ? 1 : 0;
	}

	public static int fread(CharPtr ptr, int size, int num, StreamProxy stream) {
		int num_bytes = num * size;
		byte[] bytes = new byte[num_bytes];
		try {
			int result = stream.Read(bytes, 0, num_bytes);
			for (int i = 0; i < result; i++) {
				ptr.set(i, (char)bytes[i]);
			}
			return result/size;
		}
		catch (java.lang.Exception e) {
			return 0;
		}
	}

	public static int fwrite(CharPtr ptr, int size, int num, StreamProxy stream) {
		int num_bytes = num * size;
		byte[] bytes = new byte[num_bytes];
		for (int i = 0; i < num_bytes; i++) {
			bytes[i] = (byte)ptr.get(i);
		}
		try {
			stream.Write(bytes, 0, num_bytes);
		}
		catch (java.lang.Exception e) {
			return 0;
		}
		return num;
	}

	public static int strcmp(CharPtr s1, CharPtr s2) {
		if (CharPtr.isEqual(s1, s2)) {
			return 0;
		}
		if (CharPtr.isEqual(s1, null)) {
			return -1;
		}
		if (CharPtr.isEqual(s2, null)) {
			return 1;
		}

		for (int i = 0; ; i++) {
			if (s1.get(i) != s2.get(i)) {
				if (s1.get(i) < s2.get(i)) {
					return -1;
				}
				else {
					return 1;
				}
			}
			if (s1.get(i) == '\0') {
				return 0;
			}
		}
	}

	public static CharPtr fgets(CharPtr str, StreamProxy stream) {
		int index = 0;
		try {
			while (true) {
				str.set(index, (char)stream.ReadByte());
				if (str.get(index) == '\n') {
					break;
				}
				if (index >= str.chars.length) {
					break;
				}
				index++;
			}
		}
		catch (java.lang.Exception e) {

		}
		return str;
	}

	public static double frexp(double x, int[] expptr) { //out
		expptr[0] = ClassType.log2(x) + 1;
		double s = x / Math.pow(2, expptr[0]);
		return s;
	}

	public static double ldexp(double x, int expptr) {
		return x * Math.pow(2, expptr);
	}

	public static CharPtr strstr(CharPtr str, CharPtr substr) {
		int index = str.toString().indexOf(substr.toString());
		if (index < 0) {
			return null;
		}
		return new CharPtr(CharPtr.plus(str, index));
	}

	public static CharPtr strrchr(CharPtr str, char ch) {
		int index = str.toString().lastIndexOf(ch);
		if (index < 0) {
			return null;
		}
		return CharPtr.plus(str, index);
	}

	public static StreamProxy fopen(CharPtr filename, CharPtr mode) {
		String str = filename.toString();
		String modeStr = "";
		for (int i = 0; mode.get(i) != '\0'; i++) {
			modeStr += mode.get(i);
		}
		try {
			StreamProxy result = new StreamProxy(str, modeStr);
			if (result.isOK) {
				return result;
			}
			else {
				return null;
			}
		}
		catch (java.lang.Exception e) {
			return null;
		}
	}

	public static StreamProxy freopen(CharPtr filename, CharPtr mode, StreamProxy stream) {
		try {
			stream.Flush();
			stream.Close();
		}
		catch (java.lang.Exception e) {

		}
		return fopen(filename, mode);
	}

	public static void fflush(StreamProxy stream) {
		stream.Flush();
	}

	public static int ferror(StreamProxy stream) {
		//FIXME:
		return 0; // todo: fix this - mjf
	}

	public static int fclose(StreamProxy stream) {
		stream.Close();
		return 0;
	}

	public static StreamProxy tmpfile() {
		//new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite);
		return StreamProxy.tmpfile();
	}

	public static int fscanf(StreamProxy f, CharPtr format, Object... argp) {
		String str = StreamProxy.ReadLine(); //FIXME: f
		return parse_scanf(str, format, argp);
	}

	public static int fseek(StreamProxy f, long offset, int origin) {
		return f.Seek(offset, origin);
	}


	public static int ftell(StreamProxy f) {
		return (int)f.getPosition();
	}

	public static int clearerr(StreamProxy f) {
		//ClassType.Assert(false, "clearerr not implemented yet - mjf");
		return 0;
	}

	public static int setvbuf(StreamProxy stream, CharPtr buffer, int mode, int size) { //uint
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

	public static void memcpy_char(char[] dst, int offset, char[] src, int length) {
		for (int i=0; i<length; i++) {
			dst[offset+i] = src[i];
		}
	}

	public static void memcpy_char(char[] dst, char[] src, int srcofs, int length) {
		for (int i = 0; i < length; i++) {
			dst[i] = src[srcofs+i];
		}
	}

	//public static void memcpy(CharPtr ptr1, CharPtr ptr2, uint size) 
	//{ 
	//	memcpy(ptr1, ptr2, (int)size); 
	//}

	public static void memcpy(CharPtr ptr1, CharPtr ptr2, int size) {
		for (int i = 0; i < size; i++) {
			ptr1.set(i, ptr2.get(i));
		}
	}

	public static Object VOID(Object f) {
		return f;
	}

	public static final double HUGE_VAL = Double.MAX_VALUE; //System.
	public static final int SHRT_MAX = Short.MAX_VALUE; //System.UInt16 - uint

	public static final int _IONBF = 0;
	public static final int _IOFBF = 1;
	public static final int _IOLBF = 2;

	public static final int SEEK_SET = 0;
	public static final int SEEK_CUR = 1;
	public static final int SEEK_END = 2;

	// one of the primary objectives of this port is to match the C version of Lua as closely as
	// possible. a key part of this is also matching the behaviour of the garbage collector, as
	// that affects the operation of things such as weak tables. in order for this to occur the
	// size of structures that are allocated must be reported as identical to their C++ equivelents.
	// that this means that variables such as global_State.totalbytes no longer indicate the true
	// amount of memory allocated.
	public static int GetUnmanagedSize(ClassType t) {
		return t.GetUnmanagedSize();
	}
}
