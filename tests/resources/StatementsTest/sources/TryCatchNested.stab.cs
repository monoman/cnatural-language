public class TryCatchNested {
	public static int test() {
		var result = 1;
		try {
			try {
				throw new java.lang.Exception();
			} catch {
				result += 2;
			}
		} catch {
			result += 3;
		}
		return result;
	}
}
