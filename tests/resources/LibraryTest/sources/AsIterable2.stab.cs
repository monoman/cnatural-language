using java.lang;
using java.util;
using stab.query;

public class AsIterable2 {
	public static string test() {
		string result = "";
		foreach (var s in Query.asIterable(new[] { "a", "b", "c" })) {
			result += s;
		}
		return result;
	}
}
