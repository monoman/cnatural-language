using java.lang;
using java.util;
using stab.query;

public class SelectInt {
	public static int test() {
		var list = new ArrayList<String> { "a", "bb", "ccc" };
		int result = 0;
		foreach (var l in list.select(p => p.length())) {
			result += l;
		}
		return result;
	}
}
