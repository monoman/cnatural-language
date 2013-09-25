using java.lang;
using java.util;

public class GenericNestedClass2 {
	public static string test() {
		var map = new HashMap<string, string>();
		map["key"] = "OK";
		Map.Entry<string, string> e = map.entrySet().iterator().next();
		return e.Value;
	}

}
