using java.lang;
using java.util;
using stab.query;

public class Count2 {
	public static int test() {
		var list = new ArrayList<String> { "a", "bb", "ccc" };
		return list.count(p => p.length() > 1);
	}
}
