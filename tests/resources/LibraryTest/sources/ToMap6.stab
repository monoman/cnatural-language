using java.lang;
using java.util;
using stab.query;

public class ToMap6 {
	public static int test() {
		var list = new ArrayList<String> { "V1", "V2", "V3" };
		var map = list.toMap(p => "K" + p.substring(1));
		if (!map.remove("K1").equals("V1")) {
			return 1;
		}
		if (map.size() != 2) {
			return 2;
		}
		return 0;
	}
}
