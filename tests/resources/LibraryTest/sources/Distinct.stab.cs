using java.lang;
using java.util;
using stab.query;

public class Distinct {
	public static int test() {
		var list = new ArrayList<String> { "a", "b", "a", "c" };
		return list.distinct().count();
	}
}
