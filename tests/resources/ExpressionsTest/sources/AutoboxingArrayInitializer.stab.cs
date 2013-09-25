using java.lang;

public class AutoboxingArrayInitializer {
	public static int test(int i) {
		Integer[] t = { i };
		return t[0];
	}
}