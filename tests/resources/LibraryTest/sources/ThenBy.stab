using java.lang;
using java.util;
using stab.query;

public class ThenBy {
	public static String test() {
		var list = new ArrayList<String> { "bbb", "aaa", "cc", "c", "bb", "a", "ccc", "aa", "b" };
		var result = "";
		foreach (var s in list.orderBy(p => Integer.valueOf(p.length())).thenBy(p => p)) {
			result += s;
		}
		return result;
	}
}
