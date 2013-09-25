using java.lang;

public class TryInFinally {
	public static int test() {
		int result = 1;
		try {
			result++;
		} finally {
			try {
				result += Integer.parseInt("3");
			} catch (Exception) {
			}
		}
		return result;
	}
}
