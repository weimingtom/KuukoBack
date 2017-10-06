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
