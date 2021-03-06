using java.lang;
using java.util;
using stab.query;

public class ToTFloatMap5 {
	public static bool test() {
		var map1 = new HashMap<string, Float> { { "K1.0", 1f }, { "K2.0", 2f }, { "K3.0", 3f }};
		var map2 = Query.asIterable(new[] { 1f, 2f, 3f }).toMap(p => "K" + p);
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
