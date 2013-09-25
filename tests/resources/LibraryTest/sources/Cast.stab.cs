using java.lang;
using java.util;
using stab.query;

public class Cast {
	public static string test() {
		var list = new ArrayList<Object> { "ab", "cd", "ef" };
		var str = "";
		foreach (var s in list.cast(typeof(string))) {
			str += s;
		}
		return str;
	}
}