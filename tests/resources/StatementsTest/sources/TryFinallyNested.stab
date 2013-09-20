using java.lang;

public class TryFinallyNested {
	public static int test() {
		int result = 0;
		try {
			try {
				throw new Exception();			
			} finally {
				result++;
			}
		} catch (Exception e) {
			result += 10;
		} finally {
			result += 100;
		}
		return result;
	}
}
