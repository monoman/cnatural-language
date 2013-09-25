public class Continue {
	public static int test() {
		int result = 0;
		for (int i = 0; i < 3; i++) {
			if (i == 1) {
				continue;
			}
			result++;
		}
		return result;
	}
}
