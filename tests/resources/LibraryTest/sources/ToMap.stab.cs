using java.lang;
using java.util;
using stab.query;

public class ToMap {
	public static bool test() {
		var list = new ArrayList<string> { "AA", "BB", "CC" };
		var map = list.toMap(p => p.substring(0, 1));
		return map["B"].equals("BB"); 
	}
}
