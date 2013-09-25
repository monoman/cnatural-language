using java.lang;
using java.util;
using stab.query;

public class ToTFloatMap3 {
	public static int test() {
		var map1 = new HashMap<String, Float> { { "K1", 1f }, { "K2", 2f }, { "K3", 3f }};
		var map2 = Query.emptyFloat().toMap(p => "");
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
