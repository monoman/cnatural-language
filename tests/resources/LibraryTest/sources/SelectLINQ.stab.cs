using java.lang;
using java.util;
using stab.query;

public class SelectLINQ {
	public static string test() {
		var list = new ArrayList<string> { "a", "b", "c" };
		var query = from s in list
					select s.toUpperCase();
		var result = "";
		foreach (var s in query) {
			result += s;
		}
		return result;
	}
}