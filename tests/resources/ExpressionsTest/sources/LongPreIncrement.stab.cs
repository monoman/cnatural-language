public class LongPreIncrement {
	public static bool test(long arg) {
		long l = ++arg;
		return arg == l;
	}
}
