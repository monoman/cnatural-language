using java.lang;
using java.util;
using stab.query;

public class SelectLINQ {
	public static String test() {
		var list = new ArrayList<String> { "a", "b", "c" };
		var query = from s in list
					select s.toUpperCase();
		var result = "";
		foreach (var s in query) {
			result += s;
		}
		return result;
	}
}