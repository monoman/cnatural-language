public class TryFinally {
	public static int test() {
		int result = 1;
		try {
			result += 2;
		} finally {
			result += 3;		
		}
		return result;
	}
}
