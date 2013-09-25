using java.lang;
using java.util;
using stab.query;

public class ToFloatTMap3 {
	public static int test() {
		var map1 = new HashMap<Float, String> { { 1f, "V1" }, { 2f, "V2" }, { 3f, "V3" }};
		var map2 = Query.empty<String>().toMap(p => 0f);
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
