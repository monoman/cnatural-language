using java.lang;
using java.util;
using stab.query;

public class ToLongTMap2 {
	public static bool test() {
		var list = new ArrayList<string> { "A", "BB", "CCC" };
		var map = list.toMap(p => (long)(p.length() - 1));
		return map.get(1L).equals("BB");
	}
}
