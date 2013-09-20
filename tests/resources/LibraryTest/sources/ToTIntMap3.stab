using java.lang;
using java.util;
using stab.query;

public class ToTIntMap3 {
	public static int test() {
		var map1 = new HashMap<String, Integer> { { "K1", 1 }, { "K2", 2 }, { "K3", 3 }};
		var map2 = Query.emptyInt().toMap(p => "");
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
