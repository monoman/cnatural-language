public class MinLongConstant {
	public static bool test() {
		long l = -9223372036854775808L;
		return l == -9223372036854775807-1l;
	}
}
