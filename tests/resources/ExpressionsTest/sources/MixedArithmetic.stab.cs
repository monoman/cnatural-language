public class MixedArithmetic {
	public static double test(byte b, short s, char c, int i, long l, float f) {
		return b + s - c * i / l % f;
	}
}
