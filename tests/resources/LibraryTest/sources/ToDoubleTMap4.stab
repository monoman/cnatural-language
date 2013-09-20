using java.lang;
using java.util;
using stab.query;

public class ToDoubleTMap4 {
	public static int test() {
		var map1 = new HashMap<Double, String> { { 1d, "V1" }, { 2d, "V2" }, { 3d, "V3" }};
		var map2 = Query.empty<String>().toMap(p => 0d);
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
