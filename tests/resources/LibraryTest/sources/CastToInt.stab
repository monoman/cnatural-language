using java.lang;
using java.util;
using stab.query;

public class CastToInt {
	public static int test() {
		Iterable<Integer> iterable = Query.range(1, 4);
		var result = 0;
		foreach (var i in iterable.castToInt()) {
			result += i;
		}
		return result;
	}
}