using java.lang;
using java.util;
using stab.query;

public class ToMap4 {
	public static int test() {
		var map1 = new HashMap<String, String> { { "K1", "V1" }, { "K2", "V2" }, { "K3", "V3" }};
		var list = new ArrayList<String> { "V1", "V2", "V3" };
		var map2 = list.toMap(p => "K" + p.substring(1));
		int i = 0;
		foreach (var v in map2.values()) {
			if (!map1.containsValue(v)) {
				return 1;
			}
			if (!map2.containsValue(v)) {
				return 2;
			}
			i++;
		}
		if (i != 3) {
			return 3;
		}
		return 0;
	}
}
