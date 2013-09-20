using java.lang;

public class TryInFinally2 {
	public static int test() {
		int result = 1;
		Object obj = null;
		try {
			result++;
		} finally {
			try {
				obj.toString();
			} catch {
				result++;
			}
		}
		return result;
	}
}
