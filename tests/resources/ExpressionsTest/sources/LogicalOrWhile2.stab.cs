public class LogicalOrWhile2 {
	public static int test(int i) {
		while (i < -1 || !(i < 10) || i == 0) {
			if (i < 0) {
				i++;
			} else {
				i--;
			}
		}
		return i;
	}
}
