public class ShortArithmetic {
	public static bool test() {
		short v = (short)0xffff;
		return ++v == 0;
	}
}