using java.lang;
using java.util;
using stab.query;

public class SelectInt2 {
	public static string test() {
		string result = "";
		foreach (var l in Query.range(0, 3).select(p => "" + (char)('a' + p))) {
			result += l;
		}
		return result;
	}
}
