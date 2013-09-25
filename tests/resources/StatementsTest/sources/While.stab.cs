public class While {
	public static int test(int arg) {
		int result = 0;
		while (arg-- > 0) {
			result++;
		}
		return result;
	}
}