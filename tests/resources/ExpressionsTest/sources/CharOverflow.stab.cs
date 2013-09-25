public class CharOverflow {
	public static bool test() {
		char v = 32767;
		return ++v == 32768;
	}
}