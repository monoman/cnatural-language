public class For3 {
	public static int test() {
		int result = 0;
		for (int i = 0; true; i++, result++) {
			if (i == 3) break;
		}
		return result;
	}
}
