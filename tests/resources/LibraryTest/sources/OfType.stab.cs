using java.lang;
using java.util;
using stab.query;

public class OfType {
	public static String test() {
		var list = new ArrayList<Object> { "ab", new Object(), "cd", "ef" };
		var result = "";
		foreach (var s in list.ofType(typeof(String))) {
			result += s;
		}
		return result;
	}
}