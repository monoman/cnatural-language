using java.lang;
using java.util;
using stab.query;

public class AsIterable2 {
	public static String test() {
		String result = "";
		foreach (var s in Query.asIterable(new[] { "a", "b", "c" })) {
			result += s;
		}
		return result;
	}
}
