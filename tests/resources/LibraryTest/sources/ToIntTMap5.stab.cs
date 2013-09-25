using java.lang;
using java.util;
using stab.query;

public class ToIntTMap5 {
	public static bool test() {
		var map1 = new HashMap<Integer, string> { { 1, "V1" }, { 2, "V2" }, { 3, "V3" }};
		var list = new ArrayList<string> { "V1", "V2", "V3" };
		var key = 1;
		var map2 = list.toMap(p => key++);
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
