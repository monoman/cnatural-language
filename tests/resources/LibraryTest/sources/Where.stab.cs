using java.lang;
using java.util;
using stab.query;

public class Where {
	public static int test() {
		var list = new ArrayList<String> { "a", "bb", "ccc" };
		return list.where(p => p.length() > 1).count();
	}
}
