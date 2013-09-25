using java.lang;
using java.util;
using stab.query;

public class ToTFloatMap4 {
	public static int test() {
		var map1 = new HashMap<String, Float> { { "K1", 1f }, { "K2", 2f }, { "K3", 3f }};
		var map2 = Query.emptyFloat().toMap(p => "");
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
