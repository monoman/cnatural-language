public class StaticArrayInitializer {
	public static int[] t1 = { 1, 2 }, t2 = { 3, 4 };

	public static int test() {
		return t1[0] + t1[1] + t2[0] + t2[1];
	}
}