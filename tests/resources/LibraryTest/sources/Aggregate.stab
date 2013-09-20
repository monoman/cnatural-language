using java.lang;
using stab.query;

public class Aggregate {
	public static String test() {
		return Query.asIterable(new long[] { 1L, 2L, 3L, 4L, 5L }).aggregate("S", (s, l) => s + l);
	}
}
