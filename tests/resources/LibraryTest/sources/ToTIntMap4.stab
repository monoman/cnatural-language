using java.lang;
using java.util;
using stab.query;

public class ToTIntMap4 {
	public static int test() {
		var map1 = new HashMap<String, Integer> { { "K1", 1 }, { "K2", 2 }, { "K3", 3 }};
		var map2 = Query.emptyInt().toMap(p => "");
		map2.putAll(map1);
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
