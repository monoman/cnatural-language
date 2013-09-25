using java.lang;
using java.util;
using stab.query;

public class ToFloatTMap2 {
	public static bool test() {
		var list = new ArrayList<string> { "A", "BB", "CCC" };
		var map = list.toMap(p => (float)(p.length() - 1));
		return map.get(1f).equals("BB");
	}
}
