using java.lang;
using java.util;
using stab.query;

public class ToDoubleTMap3 {
	public static int test() {
		var map1 = new HashMap<Double, String> { { 1d, "V1" }, { 2d, "V2" }, { 3d, "V3" }};
		var map2 = Query.empty<String>().toMap(p => 0d);
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
