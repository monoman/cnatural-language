
using java.lang;

public class DoubleLimits {
	public static bool test() {
		return 1.7976931348623157e308 == Double.MAX_VALUE
		    && 4.9e-324 == Double.MIN_VALUE;
	}
}
