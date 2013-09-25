public class ByteOverflow {
	public static bool test() {
		byte v = 127;
		return ++v == -128;
	}
}