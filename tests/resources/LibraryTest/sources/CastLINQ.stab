using java.lang;
using java.util;
using stab.query;

public class CastLINQ {
	public static String test() {
		var list = new ArrayList<Object> { "ab", "cd", "ef" };
		var query = from String s in list
					select s;
		var str = "";
		foreach (var s in query) {
			str += s;
		}
		return str;
	}
}