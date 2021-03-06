using java.lang;
using java.util;

public class GenericNestedInterface {
	public static int test() {
		var map = new HashMap<string, Object>();
		map.put("str1", new Object());
		map.put("str2", new Object());
			
		var map2 = new HashMap<string, Object>();
		foreach (var e in map.entrySet()) {
			map2.put(e.getKey(), e.getValue());
		}
		return map2.size();
	}
}