using java.lang;
using java.util;
using stab.query;

public class ToIntTMap2 {
	public static bool test() {
		var list = new ArrayList<string> { "A", "BB", "CCC" };
		var map = list.toMap(p => p.length() - 1);
		return map.get(1).equals("BB");
	}
}
