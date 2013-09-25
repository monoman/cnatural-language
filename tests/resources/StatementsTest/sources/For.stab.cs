public class For {
	public static int test(int arg) {
		int result = 0;
		for (int i = 0; i < arg; i++) {
			if ((i % 2) == 0) {
				result++;
			}
		}
		return result;
	}
}
