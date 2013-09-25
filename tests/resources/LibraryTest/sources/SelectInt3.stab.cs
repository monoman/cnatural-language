using java.lang;
using java.util;
using stab.query;

public class SelectInt3 {
	public static string test() {
		string result = "";
		foreach (var l in Query.range(1, 3).select(p => p * 2)) {
			result += l;
		}
		return result;
	}
}
