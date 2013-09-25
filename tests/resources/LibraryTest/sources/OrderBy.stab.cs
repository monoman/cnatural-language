using java.lang;
using java.util;
using stab.query;

public class OrderBy {
	public static string test() {
		var list = new ArrayList<string> { "bb", "a", "ccc" };
		var result = "";
		foreach (var s in list.orderBy(p => p)) {
			result += s;
		}
		return result;
	}
}
