package kurumi;

public final class Tools {
	public static String sprintf(String Format, Object... Parameters) {
		boolean hasFloat = false;
		if (Format.equals(LuaConf.LUA_NUMBER_FMT) && Parameters.length == 1) {
			if ((Double)Parameters[0] == ((Double)Parameters[0]).longValue()) {
				Format = "%s";
				Parameters[0] = ((Double)Parameters[0]).longValue();
			} else {
				Format = "%s";
				hasFloat = true;
			}
		} else if (Format.equals("%ld")) {
			Format = "%d";
		} else if (LuaConf.LUA_INTFRMLEN.length() > 0 && Format.contains(LuaConf.LUA_INTFRMLEN)) {
			//FIXME:???, string.format("%08X  ",offset) changed to %08lX
			//see str_format, addintlen, LuaConf.LUA_INTFRMLEN
			Format = Format.replace(LuaConf.LUA_INTFRMLEN, ""); 
		} 
		String result = String.format(Format, Parameters);
		if (hasFloat) {
			String[] subResults = result.split("\\.");
			if (subResults.length == 2 && subResults[1].length() > 13) {
				result = String.format(LuaConf.LUA_NUMBER_FMT, Parameters);
			}
		}
		return result;
	}
	public static void printf(String Format, Object... Parameters) {
		System.out.print(Tools.sprintf(Format, Parameters));
	}
}
