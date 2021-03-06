using java.lang;
using java.util;
using stab.query;

public class ToTDoubleMap {
	public static bool test() {
		var map = Query.asIterable(new[] { 1d, 2d, 3d }).toMap(p => "K" + p);
		return map.containsValue(1d) &&
				map.containsValue(2d) &&
				map.containsValue(3d) &&
				!map.containsValue(4d) &&
				map.containsKey("K1.0") &&
				map.containsKey("K2.0") &&
				map.containsKey("K3.0") &&
				!map.containsKey("K4.0");
	}
}
