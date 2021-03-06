package kurumi;

//
// ** $Id: llex.c,v 2.20.1.1 2007/12/27 13:02:25 roberto Exp $
// ** Lexical Analyzer
// ** See Copyright Notice in lua.h
// 
//using TValue = Lua.TValue;
//using lua_Number = System.Double;

public class LuaLex {
	public static final int FIRST_RESERVED = 257;

	// maximum length of a reserved word 
	public static final int TOKEN_LEN = 9; // "function"

	/*
	 * WARNING: if you change the order of this enumeration,
	 * grep "ORDER RESERVED"
	 */
	public static class RESERVED {
		/* terminal symbols denoted by reserved words */
		public static final int TK_AND = LuaLex.FIRST_RESERVED;
		public static final int TK_BREAK = LuaLex.FIRST_RESERVED + 1;
		public static final int TK_DO = LuaLex.FIRST_RESERVED + 2;
		public static final int TK_ELSE = LuaLex.FIRST_RESERVED + 3;
		public static final int TK_ELSEIF = LuaLex.FIRST_RESERVED + 4;
		public static final int TK_END = LuaLex.FIRST_RESERVED + 5;
		public static final int TK_FALSE = LuaLex.FIRST_RESERVED + 6;
		public static final int TK_FOR = LuaLex.FIRST_RESERVED + 7;
		public static final int TK_FUNCTION = LuaLex.FIRST_RESERVED + 8;
		public static final int TK_IF = LuaLex.FIRST_RESERVED + 9;
		public static final int TK_IN = LuaLex.FIRST_RESERVED + 10;
		public static final int TK_LOCAL = LuaLex.FIRST_RESERVED + 11;
		public static final int TK_NIL = LuaLex.FIRST_RESERVED + 12;
		public static final int TK_NOT = LuaLex.FIRST_RESERVED + 13;
		public static final int TK_OR = LuaLex.FIRST_RESERVED + 14;
		public static final int TK_REPEAT = LuaLex.FIRST_RESERVED + 15;
		public static final int TK_RETURN = LuaLex.FIRST_RESERVED + 16;
		public static final int TK_THEN = LuaLex.FIRST_RESERVED + 17;
		public static final int TK_TRUE = LuaLex.FIRST_RESERVED + 18;
		public static final int TK_UNTIL = LuaLex.FIRST_RESERVED + 19;
		public static final int TK_WHILE = LuaLex.FIRST_RESERVED + 20;
		/* other terminal symbols */
		public static final int TK_CONCAT = LuaLex.FIRST_RESERVED + 21;
		public static final int TK_DOTS = LuaLex.FIRST_RESERVED + 22;
		public static final int TK_EQ = LuaLex.FIRST_RESERVED + 23;
		public static final int TK_GE = LuaLex.FIRST_RESERVED + 24;
		public static final int TK_LE = LuaLex.FIRST_RESERVED + 25;
		public static final int TK_NE = LuaLex.FIRST_RESERVED + 26;
		public static final int TK_NUMBER = LuaLex.FIRST_RESERVED + 27;
		public static final int TK_NAME = LuaLex.FIRST_RESERVED + 28;
		public static final int TK_STRING = LuaLex.FIRST_RESERVED + 29;
		public static final int TK_EOS = LuaLex.FIRST_RESERVED + 30;
	}
	
	// number of reserved words 
	public static final int NUM_RESERVED = (int)RESERVED.TK_WHILE - FIRST_RESERVED + 1;
	
	public static class SemInfo
	{
		public double r;  /*Double*/ /*lua_Number*/
		public LuaObject.TString ts;
		
		public SemInfo() 
		{
			
		}
		
		public SemInfo(SemInfo copy)
		{
			this.r = copy.r;
			this.ts = copy.ts;
		}
	}  /* semantics information */
	
	public static class Token
	{
		public int token;
		public LuaLex.SemInfo seminfo = new LuaLex.SemInfo();
		
		public Token()
		{
			
		}
		
		public Token(Token copy)
		{
			this.token = copy.token;
			this.seminfo = new LuaLex.SemInfo(copy.seminfo);
		}
	}
	
	public static class LexState {
		public int current;  /* current character (charint) */
		public int linenumber;  /* input line counter */
		public int lastline;  /* line of last token `consumed' */
		public Token t = new Token();  /* current token */
		public Token lookahead = new Token();  /* look ahead token */
		public LuaParser.FuncState fs;  /* `FuncState' is private to the parser */
		public LuaState.lua_State L;
		public LuaZIO.ZIO z;  /* input stream */
		public LuaZIO.Mbuffer buff;  /* buffer for tokens */
		public LuaObject.TString source;  /* current source name */
		public char decpoint;  /* locale decimal point */
	}
	
	public static void next(LexState ls) {
		ls.current = LuaZIO.zgetc(ls.z);
	}

	public static boolean currIsNewline(LexState ls) {
		return (ls.current == '\n' || ls.current == '\r');
	}

	// ORDER RESERVED 
	public static final String[] luaX_tokens = { "and", "break", "do", "else", "elseif", "end", "false", "for", "function", "if", "in", "local", "nil", "not", "or", "repeat", "return", "then", "true", "until", "while", "..", "...", "==", ">=", "<=", "~=", "<number>", "<name>", "<string>", "<eof>" };

	public static void save_and_next(LexState ls) {
		save(ls, ls.current);
		next(ls);
	}

	private static void save(LexState ls, int c) {
		LuaZIO.Mbuffer b = ls.buff;
		if (b.n + 1 > b.buffsize) {
			int newsize; //uint
			if (b.buffsize >= LuaLimits.MAX_SIZET / 2) {
				luaX_lexerror(ls, CLib.CharPtr.toCharPtr("lexical element too long"), 0);
			}
			newsize = b.buffsize * 2;
			LuaZIO.luaZ_resizebuffer(ls.L, b, (int)newsize);
		}
		b.buffer.set(b.n++, (char)c);
	}

	public static void luaX_init(LuaState.lua_State L) {
		int i;
		for (i = 0; i < NUM_RESERVED; i++) {
			LuaObject.TString ts = LuaString.luaS_new(L, CLib.CharPtr.toCharPtr(luaX_tokens[i]));
			LuaString.luaS_fix(ts); // reserved words are never collected 
			LuaLimits.lua_assert(luaX_tokens[i].length() + 1 <= TOKEN_LEN);
			ts.getTsv().reserved = LuaLimits.cast_byte(i + 1); // reserved word 
		}
	}

	public static final int MAXSRC = 80;

	public static CLib.CharPtr luaX_token2str(LexState ls, int token) {
		if (token < FIRST_RESERVED) {
			LuaLimits.lua_assert(token == (byte)token);
			return (CLib.iscntrl(token)) ? LuaObject.luaO_pushfstring(ls.L, CLib.CharPtr.toCharPtr("char(%d)"), token) : LuaObject.luaO_pushfstring(ls.L, CLib.CharPtr.toCharPtr("%c"), token);
		}
		else {
			return CLib.CharPtr.toCharPtr(luaX_tokens[(int)token - FIRST_RESERVED]);
		}
	}

	public static CLib.CharPtr txtToken(LexState ls, int token) {
		switch (token) {
			case (int)RESERVED.TK_NAME:
			case (int)RESERVED.TK_STRING:
			case (int)RESERVED.TK_NUMBER: {
					save(ls, '\0');
					return LuaZIO.luaZ_buffer(ls.buff);
				}
			default: {
					return luaX_token2str(ls, token);
				}
		}
	}

	public static void luaX_lexerror(LexState ls, CLib.CharPtr msg, int token) {
		CLib.CharPtr buff = CLib.CharPtr.toCharPtr(new char[MAXSRC]);
		LuaObject.luaO_chunkid(buff, LuaObject.getstr(ls.source), MAXSRC);
		msg = LuaObject.luaO_pushfstring(ls.L, CLib.CharPtr.toCharPtr("%s:%d: %s"), buff, ls.linenumber, msg);
		if (token != 0) {
			LuaObject.luaO_pushfstring(ls.L, CLib.CharPtr.toCharPtr("%s near " + LuaConf.getLUA_QS()), msg, txtToken(ls, token));
		}
		LuaDo.luaD_throw(ls.L, Lua.LUA_ERRSYNTAX);
	}

	public static void luaX_syntaxerror(LexState ls, CLib.CharPtr msg) {
		luaX_lexerror(ls, msg, ls.t.token);
	}

	public static LuaObject.TString luaX_newstring(LexState ls, CLib.CharPtr str, int l) { //uint
		LuaState.lua_State L = ls.L;
		LuaObject.TString ts = LuaString.luaS_newlstr(L, str, l);
		LuaObject.TValue o = LuaTable.luaH_setstr(L, ls.fs.h, ts); // entry for `str' 
		if (LuaObject.ttisnil(o)) {
			LuaObject.setbvalue(o, 1); // make sure `str' will not be collected 
		}
		return ts;
	}

	private static void inclinenumber(LexState ls) {
		int old = ls.current;
		LuaLimits.lua_assert(currIsNewline(ls));
		next(ls); // skip `\n' or `\r' 
		if (currIsNewline(ls) && ls.current != old) {
			next(ls); // skip `\n\r' or `\r\n' 
		}
		if (++ls.linenumber >= LuaLimits.MAX_INT) {
			luaX_syntaxerror(ls, CLib.CharPtr.toCharPtr("chunk has too many lines"));
		}
	}

	public static void luaX_setinput(LuaState.lua_State L, LexState ls, LuaZIO.ZIO z, LuaObject.TString source) {
		ls.decpoint = '.';
		ls.L = L;
		ls.lookahead.token = (int)RESERVED.TK_EOS; // no look-ahead token 
		ls.z = z;
		ls.fs = null;
		ls.linenumber = 1;
		ls.lastline = 1;
		ls.source = source;
		LuaZIO.luaZ_resizebuffer(ls.L, ls.buff, LuaLimits.LUA_MINBUFFER); // initialize buffer 
		next(ls); // read first char 
	}

//        
//		 ** =======================================================
//		 ** LEXICAL ANALYZER
//		 ** =======================================================
//		 
	private static int check_next(LexState ls, CLib.CharPtr set) {
		if (CLib.CharPtr.isEqual(CLib.strchr(set, (char)ls.current), null)) {
			return 0;
		}
		save_and_next(ls);
		return 1;
	}

	private static void buffreplace(LexState ls, char from, char to) {
		int n = LuaZIO.luaZ_bufflen(ls.buff); //uint
		CLib.CharPtr p = LuaZIO.luaZ_buffer(ls.buff);
		while ((n--) != 0) {
			if (p.get(n) == from) {
				p.set(n, to);
			}
		}
	}

	private static void trydecpoint(LexState ls, SemInfo seminfo) {
		// format error: try to update decimal point separator 
		// todo: add proper support for localeconv - mjf
		//lconv cv = localeconv();
		char old = ls.decpoint;
		ls.decpoint = '.'; // (cv ? cv.decimal_point[0] : '.');
		buffreplace(ls, old, ls.decpoint); // try updated decimal separator 
		double[] r = new double[1];
		r[0] = seminfo.r;
		int ret = LuaObject.luaO_str2d(LuaZIO.luaZ_buffer(ls.buff), r);
		seminfo.r = r[0];
		if (ret == 0) {
			// format error with correct decimal point: no more options 
			buffreplace(ls, ls.decpoint, '.'); // undo change (for error message) 
			luaX_lexerror(ls, CLib.CharPtr.toCharPtr("malformed number"), (int)RESERVED.TK_NUMBER);
		}
	}

	// LUA_NUMBER 
	private static void read_numeral(LexState ls, SemInfo seminfo) {
		LuaLimits.lua_assert(CLib.isdigit(ls.current));
		do {
			save_and_next(ls);
		} while (CLib.isdigit(ls.current) || ls.current == '.');
		if (check_next(ls, CLib.CharPtr.toCharPtr("Ee")) != 0) { // `E'? 
			check_next(ls, CLib.CharPtr.toCharPtr("+-")); // optional exponent sign 
		}
		while (CLib.isalnum(ls.current) || ls.current == '_') {
			save_and_next(ls);
		}
		save(ls, '\0');
		buffreplace(ls, '.', ls.decpoint); // follow locale for decimal point 
		double[] r = new double[1];
		r[0] = seminfo.r;
		int ret = LuaObject.luaO_str2d(LuaZIO.luaZ_buffer(ls.buff), r);
		seminfo.r = r[0];
		if (ret == 0) { // format error? 
			trydecpoint(ls, seminfo); // try to update decimal point separator 
		}
	}

	private static int skip_sep(LexState ls) {
		int count = 0;
		int s = ls.current;
		LuaLimits.lua_assert(s == '[' || s == ']');
		save_and_next(ls);
		while (ls.current == '=') {
			save_and_next(ls);
			count++;
		}
		return (ls.current == s) ? count : (-count) - 1;
	}

	private static void read_long_string(LexState ls, SemInfo seminfo, int sep) {
		//int cont = 0;
		//(void)(cont);  /* avoid warnings when `cont' is not used */
		save_and_next(ls); // skip 2nd `[' 
		if (currIsNewline(ls)) { // string starts with a newline? 
			inclinenumber(ls); // skip it 
		}
		for (;;) {
			boolean endloop = false;
			switch (ls.current) {
				case LuaZIO.EOZ: {
						luaX_lexerror(ls, (seminfo != null) ? CLib.CharPtr.toCharPtr("unfinished long string") : CLib.CharPtr.toCharPtr("unfinished long comment"), (int)RESERVED.TK_EOS);
						break; // to avoid warnings 
					}
				///#if LUA_COMPAT_LSTR
//					case '[': 
//						{
//							if (skip_sep(ls) == sep) 
//							{
//								save_and_next(ls);  /* skip 2nd `[' */
//								cont++;
//								//#if LUA_COMPAT_LSTR
//								if (sep == 0)
//								{
//									luaX_lexerror(ls, "nesting of [[...]] is deprecated", '[');
//								}
//								//#endif
//							}
//							break;
//						}
				///#endif
				case ']': {
						if (skip_sep(ls) == sep) {
							save_and_next(ls); // skip 2nd `]' 
							///#if defined(LUA_COMPAT_LSTR) && LUA_COMPAT_LSTR == 2
							//          cont--;
							//          if (sep == 0 && cont >= 0) break;
							///#endif
							//goto endloop;
							endloop = true;
							break;
						}
						break;
					}
				case '\n':
				case '\r': {
						save(ls, '\n');
						inclinenumber(ls);
						if (seminfo == null) {
							LuaZIO.luaZ_resetbuffer(ls.buff); // avoid wasting space 
						}
						break;
					}
				default: {
						if (seminfo != null) {
							save_and_next(ls);
						}
						else {
							next(ls);
						}
					}
					break;
			}
			if (endloop) {
				break;
			}
		}
//endloop:
		if (seminfo != null) {
			seminfo.ts = luaX_newstring(ls, CLib.CharPtr.plus(LuaZIO.luaZ_buffer(ls.buff), (2 + sep)), (LuaZIO.luaZ_bufflen(ls.buff) - 2 * (2 + sep))); //(uint)
		}
	}


	private static void read_string(LexState ls, int del, SemInfo seminfo) {
		save_and_next(ls);
		while (ls.current != del) {
			switch (ls.current) {
				case LuaZIO.EOZ: {
						luaX_lexerror(ls, CLib.CharPtr.toCharPtr("unfinished string"), (int)RESERVED.TK_EOS);
						continue; // to avoid warnings 
					}
				case '\n':
				case '\r': {
						luaX_lexerror(ls, CLib.CharPtr.toCharPtr("unfinished string"), (int)RESERVED.TK_STRING);
						continue; // to avoid warnings 
					}
				case '\\': {
						int c;
						next(ls); // do not save the `\' 
						switch (ls.current) {
							case 'a': {
									c = '\u0007'; //'\a'; FIXME:
									break;
								}
							case 'b': {
									c = '\b';
									break;
								}
							case 'f': {
									c = '\f';
									break;
								}
							case 'n': {
									c = '\n';
									break;
								}
							case 'r': {
									c = '\r';
									break;
								}
							case 't': {
									c = '\t';
									break;
								}
							case 'v': {
									c = '\u000B'; //'\v'; FIXME:
									break;
								}
							case '\n': // go through 
							case '\r': {
									save(ls, '\n');
									inclinenumber(ls);
									continue;
								}
							case LuaZIO.EOZ: {
									continue; // will raise an error next loop 
								}
							default: {
									if (!CLib.isdigit(ls.current)) {
										save_and_next(ls); // handles \\, \", \', and \? 
									}
									else {
										// \xxx 
										int i = 0;
										c = 0;
										do {
											c = 10*c + (ls.current-'0');
											next(ls);
										} while (++i < 3 && CLib.isdigit(ls.current));
										//System.Byte.MaxValue
										if (c > Byte.MAX_VALUE) {
											luaX_lexerror(ls, CLib.CharPtr.toCharPtr("escape sequence too large"), (int)RESERVED.TK_STRING);
										}
										save(ls, c);
									}
									continue;
								}
						}
						save(ls, c);
						next(ls);
						continue;
					}
				default: {
						save_and_next(ls);
						break;
					}
			}
		}
		save_and_next(ls); // skip delimiter 
		seminfo.ts = luaX_newstring(ls, CLib.CharPtr.plus(LuaZIO.luaZ_buffer(ls.buff), 1), LuaZIO.luaZ_bufflen(ls.buff) - 2);
	}

	private static int llex(LexState ls, SemInfo seminfo) {
		LuaZIO.luaZ_resetbuffer(ls.buff);
		for (;;) {
			switch (ls.current) {
				case '\n':
				case '\r': {
						inclinenumber(ls);
						continue;
					}
				case '-': {
						next(ls);
						if (ls.current != '-') {
							return '-';
						}
						// else is a comment 
						next(ls);
						if (ls.current == '[') {
							int sep = skip_sep(ls);
							LuaZIO.luaZ_resetbuffer(ls.buff); // `skip_sep' may dirty the buffer 
							if (sep >= 0) {
								read_long_string(ls, null, sep); // long comment 
								LuaZIO.luaZ_resetbuffer(ls.buff);
								continue;
							}
						}
						// else short comment 
						while (!currIsNewline(ls) && ls.current != LuaZIO.EOZ) {
							next(ls);
						}
						continue;
					}
				case '[': {
						int sep = skip_sep(ls);
						if (sep >= 0) {
							read_long_string(ls, seminfo, sep);
							return (int)RESERVED.TK_STRING;
						}
						else if (sep == -1) {
							return '[';
						}
						else {
							luaX_lexerror(ls, CLib.CharPtr.toCharPtr("invalid long string delimiter"), (int)RESERVED.TK_STRING);
						}
					}
					break;
				case '=': {
						next(ls);
						if (ls.current != '=') {
							return '=';
						}
						else {
							next(ls);
							return (int)RESERVED.TK_EQ;
						}
					}
				case '<': {
						next(ls);
						if (ls.current != '=') {
							return '<';
						}
						else {
							next(ls);
							return (int)RESERVED.TK_LE;
						}
					}
				case '>': {
						next(ls);
						if (ls.current != '=') {
							return '>';
						}
						else {
							next(ls);
							return (int)RESERVED.TK_GE;
						}
					}
				case '~': {
						next(ls);
						if (ls.current != '=') {
							return '~';
						}
						else {
							next(ls);
							return (int)RESERVED.TK_NE;
						}
					}
				case '"':
				case '\'': {
						read_string(ls, ls.current, seminfo);
						return (int)RESERVED.TK_STRING;
					}
				case '.': {
						save_and_next(ls);
						if (check_next(ls, CLib.CharPtr.toCharPtr(".")) != 0) {
							if (check_next(ls, CLib.CharPtr.toCharPtr(".")) != 0) {
								return (int)RESERVED.TK_DOTS; //... 
							}
							else {
								return (int)RESERVED.TK_CONCAT; //.. 
							}
						}
						else if (!CLib.isdigit(ls.current)) {
							return '.';
						}
						else {
							read_numeral(ls, seminfo);
							return (int)RESERVED.TK_NUMBER;
						}
					}
				case LuaZIO.EOZ: {
						return (int)RESERVED.TK_EOS;
					}
				default: {
						if (CLib.isspace(ls.current)) {
							LuaLimits.lua_assert(!currIsNewline(ls));
							next(ls);
							continue;
						}
						else if (CLib.isdigit(ls.current)) {
							read_numeral(ls, seminfo);
							return (int)RESERVED.TK_NUMBER;
						}
						else if (CLib.isalpha(ls.current) || ls.current == '_') {
							// identifier or reserved word 
							LuaObject.TString ts;
							do {
								save_and_next(ls);
							} while (CLib.isalnum(ls.current) || ls.current == '_');
							ts = luaX_newstring(ls, LuaZIO.luaZ_buffer(ls.buff), LuaZIO.luaZ_bufflen(ls.buff));
							if (ts.getTsv().reserved > 0) { // reserved word? 
								return ts.getTsv().reserved - 1 + FIRST_RESERVED;
							}
							else {
								seminfo.ts = ts;
								return (int)RESERVED.TK_NAME;
							}
						}
						else {
							int c = ls.current;
							next(ls);
							return c; // single-char tokens (+ - /...) 
						}
					}
			}
		}
	}

	public static void luaX_next(LexState ls) {
		ls.lastline = ls.linenumber;
		if (ls.lookahead.token != (int)RESERVED.TK_EOS) {
			// is there a look-ahead token? 
			ls.t = new Token(ls.lookahead); // use this one 
			ls.lookahead.token = (int)RESERVED.TK_EOS; // and discharge it 
		}
		else {
			ls.t.token = llex(ls, ls.t.seminfo); // read next token 
		}
	}

	public static void luaX_lookahead(LexState ls) {
		LuaLimits.lua_assert(ls.lookahead.token == (int)RESERVED.TK_EOS);
		ls.lookahead.token = llex(ls, ls.lookahead.seminfo);
	}
}