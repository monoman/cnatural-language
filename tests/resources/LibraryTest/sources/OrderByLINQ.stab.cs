using java.lang;
using java.util;
using stab.query;

public class OrderByLINQ {
	public static string test() {
		var list = new ArrayList<string> { "bb", "a", "ccc" };
		var query = from s in list
					orderby s
					select s;
		var result = "";
		foreach (var s in query) {
			result += s;
		}
		return result;
	}
}
