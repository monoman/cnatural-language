public class TryFinallyGoto {
	public static int test() {
		int result = 1;
		try {
			goto end;
		} finally {
			result++;
		}
		end:
		return result;
	}
}
