package kurumi;

import java.io.*;

public class StreamProxy {
	private final static int TYPE_FILE = 0;
	private final static int TYPE_STDOUT = 1;
	private final static int TYPE_STDIN = 2;
	private final static int TYPE_STDERR = 3;
	public int type = TYPE_FILE;
	public boolean isOK = false;
	private RandomAccessFile _file = null;
	
	private StreamProxy() {
		this.isOK = false;
	}

	public StreamProxy(String path, String modeStr) {
		this.isOK = false;
		try {
			if (modeStr != null) {
				modeStr = modeStr.replace("b", "");
				if (modeStr.contains("w")) {
					modeStr = "rw";
				}
			}
			this._file = new RandomAccessFile(path, modeStr);
			this.isOK = true;
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		}
		this.type = TYPE_FILE;
	}

	public final void Flush() {
		if (this.type == TYPE_STDOUT) {
			//RandomAccessFile flush not need ?
		}
	}

	public final void Close() {
		if (this.type == TYPE_STDOUT) {
			if (this._file != null) {
				try {
					this._file.close();
				} catch (IOException e) {
					e.printStackTrace();
				}
				this._file = null;
			}
		}		
	}

	public final void Write(byte[] buffer, int offset, int count) {
		if (this.type == TYPE_STDOUT) {
			System.out.print(new String(buffer, offset, count));
		} else if (this.type == TYPE_STDERR) {
			System.err.print(new String(buffer, offset, count));
		} else if (this.type == TYPE_FILE) {
			if (this._file != null) {
				try {
					this._file.write(buffer, offset, count); //FIXME:don't use writeBytes(String s)
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		} else {
			//FIXME:TODO
		}
	}

	public final int Read(byte[] buffer, int offset, int count) {
		if (type == TYPE_FILE) {
			if (this._file != null) {
				try {
					int result = this._file.read(buffer, offset, count);
					return result < 0 ? 0 : result;
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		} else if (this.type == TYPE_STDIN) {
			//don't use System.in.read(), windows seem eat \r if use pipe redirect input
//			int size = 0;
//			try {
//				for (int i = offset; i < offset + count; ++i) {
//					 int result = System.in.read();
//					 if (result < 0) {
//						 break;
//					 }
//					 buffer[offset + i] = (byte)result;
//					 size++;
//				}
//			}
//			catch (IOException e) {
//				e.printStackTrace();
//			}
//			return size;
			try {
				int result = System.in.read(buffer, offset, count);
				return result < 0 ? 0 : result;
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
		return 0;
	}

	public final int Seek(long offset, int origin) {
		if (type == TYPE_FILE) {
			if (this._file != null) {
				//CLib.SEEK_SET, 
				//CLib.SEEK_CUR, 
				//CLib.SEEK_END 
				long pos = -1;
				if (origin == CLib.SEEK_CUR) {
					pos = offset;
				} else if (origin == CLib.SEEK_CUR) {
					try {
						pos = this._file.getFilePointer() + offset;
					} catch (IOException e) {
						e.printStackTrace();
					}
				} else if (origin == CLib.SEEK_END) {
					try {
						pos = this._file.length() + offset;
					} catch (IOException e) {
						e.printStackTrace();
					}					
				}
				try {
					this._file.seek(pos);
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		}
		return 0;
	}

	public final int ReadByte() {
		if (type == TYPE_STDIN) {
			try {
				return System.in.read();
			} catch (IOException e) {
				e.printStackTrace();
			}
			return 0;
		} else if (type == TYPE_FILE) {
			if (this._file != null) {
				try {
					return this._file.read();
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
			return -1;
		} else {
			return 0;
		}
	}

	public final void ungetc(int c) {
		if (type == TYPE_FILE) {
			if (this._file != null) {
				try {
					this._file.seek(this._file.getFilePointer() - 1);
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		}
	}

	public final long getPosition() {
		if (type == TYPE_FILE) {
			if (this._file != null) {
				try {
					return this._file.getFilePointer();
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		}
		return 0;
	}

	public final boolean isEof() {
		if (type == TYPE_FILE) {
			if (this._file != null) {
				try {
					return this._file.getFilePointer() >= this._file.length();
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		}
		return true;
	}

	//--------------------------------------

	public static StreamProxy tmpfile() {
		StreamProxy result = new StreamProxy();
		return result;
	}

	public static StreamProxy OpenStandardOutput() {
		StreamProxy result = new StreamProxy();
		result.type = TYPE_STDOUT;
		result.isOK = true;
		return result;
	}

	public static StreamProxy OpenStandardInput() {
		StreamProxy result = new StreamProxy();
		result.type = TYPE_STDIN;
		result.isOK = true;
		return result;
	}

	public static StreamProxy OpenStandardError() {
		StreamProxy result = new StreamProxy();
		result.type = TYPE_STDERR;
		result.isOK = true;
		return result;
	}

	public static String GetCurrentDirectory() {
		File directory = new File("");
		return directory.getAbsolutePath();
	}

	public static void Delete(String path) {
		new File(path).delete();
	}

	public static void Move(String path1, String path2) {
		new File(path1).renameTo(new File(path2));
	}

	public static String GetTempFileName() {
		try {
			return File.createTempFile("abc", ".tmp").getAbsolutePath();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return null;
	}

	public static String ReadLine() {
		BufferedReader in = new BufferedReader(new InputStreamReader(System.in));
		try {
			return in.readLine();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return null;
	}

	public static void Write(String str) {
		System.out.print(str);
	}

	public static void WriteLine() {
		System.out.println();
	}

	public static void ErrorWrite(String str) {
		System.err.print(str);
		System.err.flush();
	}
}
