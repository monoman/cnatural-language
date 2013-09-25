using java.lang;
using java.util;
using stab.query;

public class Select {
	public static string test() {
		var list = new ArrayList<string> { "a", "b", "c" };
		string result = "";
		foreach (var s in list.select(p => p.toUpperCase())) {
			result += s;
		}
		return result;
	}
}