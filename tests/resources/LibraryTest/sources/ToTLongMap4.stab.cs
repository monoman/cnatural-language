using java.lang;
using java.util;
using stab.query;

public class ToTLongMap4 {
	public static int test() {
		var map1 = new HashMap<String, Long> { { "K1", 1L }, { "K2", 2L }, { "K3", 3L }};
		var map2 = Query.emptyLong().toMap(p => "");
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
