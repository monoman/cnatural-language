public class Factorial {
	public static int test() {
		return factorial(5);
	}

	public static int factorial(int n) {
		if (n < 1) {
			return 1;
		} else {
			return n * factorial(n - 1);
		}
	}
}
