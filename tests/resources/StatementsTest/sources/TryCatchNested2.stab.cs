public class TryCatchNested2 {
	public static int test() {
		var result = 1;
		try {
			try {
				throw new java.lang.Exception();
			} catch {
				result += 2;
				throw;
			}
		} catch {
			result += 3;
		}
		return result;
	}
}
