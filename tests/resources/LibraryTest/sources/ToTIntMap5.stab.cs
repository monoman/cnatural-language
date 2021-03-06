using java.lang;
using java.util;
using stab.query;

public class ToTIntMap5 {
	public static bool test() {
		var map1 = new HashMap<string, Integer> { { "K1", 1 }, { "K2", 2 }, { "K3", 3 }};
		var map2 = Query.asIterable(new[] { 1, 2, 3 }).toMap(p => "K" + p);
		int i = 0;
		foreach (var k in map2.keySet()) {
			if (!map1[k].equals(map2.get(k))) {
				return false;
			}
			i++;
		}
		return map2.size() == 3 && i == 3;
	}
}
