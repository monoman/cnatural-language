
using java.lang;

public class FloatLimits2 {
	public static bool test() {
		return 1f/0f == Float.POSITIVE_INFINITY && -1/0f == Float.NEGATIVE_INFINITY;
	}
}