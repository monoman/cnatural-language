public class Gcd {
	public static int test(int a, int b)
	{
		int r;          // Hold the remainder

		do {
			r = a % b;
			a = b;
			b = r;
		}
		while (r > 0);

		return a;
	}
}
