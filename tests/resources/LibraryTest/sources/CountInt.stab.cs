using java.lang;
using stab.query;

public class CountInt {
	public static int test() {
		return Query.range(-1, 3).count(p => p >= 0);
	}
}
