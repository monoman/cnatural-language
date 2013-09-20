using java.lang;
using java.util;
using stab.query;

public class Select {
	public static String test() {
		var list = new ArrayList<String> { "a", "b", "c" };
		String result = "";
		foreach (var s in list.select(p => p.toUpperCase())) {
			result += s;
		}
		return result;
	}
}