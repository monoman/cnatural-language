public class LogicalOrWhile {
	public static int test(int i) {
		while (i < -1 || !(i < 10)) {
			if (i < 0) {
				i++;
			} else {
				i--;
			}
		}
		return i;
	}
}
