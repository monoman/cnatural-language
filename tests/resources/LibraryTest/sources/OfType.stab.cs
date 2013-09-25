using java.lang;
using java.util;
using stab.query;

public class OfType {
	public static string test() {
		var list = new ArrayList<Object> { "ab", new Object(), "cd", "ef" };
		var result = "";
		foreach (var s in list.ofType(typeof(string))) {
			result += s;
		}
		return result;
	}
}