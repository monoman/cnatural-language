using java.lang;
using java.util;
using stab.query;

public class ToIntTMap4 {
	public static int test() {
		var map1 = new HashMap<Integer, String> { { 1, "V1" }, { 2, "V2" }, { 3, "V3" }};
		var map2 = Query.empty<String>().toMap(p => 0);
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
