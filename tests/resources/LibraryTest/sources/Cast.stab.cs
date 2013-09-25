using java.lang;
using java.util;
using stab.query;

public class Cast {
	public static String test() {
		var list = new ArrayList<Object> { "ab", "cd", "ef" };
		var str = "";
		foreach (var s in list.cast(typeof(String))) {
			str += s;
		}
		return str;
	}
}