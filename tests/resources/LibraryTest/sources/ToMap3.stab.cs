using java.lang;
using java.util;
using stab.query;

public class ToMap3 {
	public static int test() {
		var map1 = new HashMap<String, String> { { "K1", "V1" }, { "K2", "V2" }, { "K3", "V3" }};
		var map2 = Query.empty<String>().toMap(p => p);
		map2.putAll(map1);
		int i = 0;
		foreach (var e in map2.entrySet()) {
			if (map1[e.Key] != e.Value) {
				return 1;
			}
			i++;
		}
		if (i != 3) {
			return 2;
		}
		return 0;
	}
}
