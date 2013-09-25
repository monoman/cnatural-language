using java.lang;
using java.util;
using stab.query;

public class CastLINQ {
	public static string test() {
		var list = new ArrayList<Object> { "ab", "cd", "ef" };
		var query = from string s in list
					select s;
		var str = "";
		foreach (var s in query) {
			str += s;
		}
		return str;
	}
}