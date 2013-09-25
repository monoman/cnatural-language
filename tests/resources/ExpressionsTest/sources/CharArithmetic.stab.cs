public class CharArithmetic {
	public static bool test() {
		char v = '\uFFFF';
		return ++v == 0;
	}
}