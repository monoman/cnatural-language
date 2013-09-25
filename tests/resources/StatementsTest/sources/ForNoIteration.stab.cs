public class ForNoIteration {
	public static int test(int arg) {
		int result = 0;
		for (int i = 0; i < arg; ) {
			if ((i % 2) == 0) {
				result++;
			}
			i++;
		}
		return result;
	}
}
