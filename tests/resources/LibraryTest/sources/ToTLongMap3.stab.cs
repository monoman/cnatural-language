using java.lang;
using java.util;
using stab.query;

public class ToTLongMap3 {
	public static int test() {
		var map1 = new HashMap<String, Long> { { "K1", 1L }, { "K2", 2L }, { "K3", 3L }};
		var map2 = Query.emptyLong().toMap(p => "");
		map2.putAll(map1);
		int i = 0;
		foreach (var e in map2.entrySet()) {
			if (!map1[e.Key].equals(e.Value)) {
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
