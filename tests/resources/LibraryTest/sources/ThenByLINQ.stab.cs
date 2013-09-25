using java.lang;
using java.util;
using stab.query;

public class ThenByLINQ {
	public static string test() {
		var list = new ArrayList<string> { "bbb", "aaa", "cc", "c", "bb", "a", "ccc", "aa", "b" };
		var query = from s in list
					orderby Integer.valueOf(s.length()), s
					select s;
		var result = "";
		foreach (var s in query) {
			result += s;
		}
		return result;
	}
}
