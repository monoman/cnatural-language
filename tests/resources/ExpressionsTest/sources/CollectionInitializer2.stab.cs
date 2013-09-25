using java.lang;
using java.util;

public class CollectionInitializer2 {
	public static int test() {
		var map = new HashMap<String, String> { { "a", "b" }, { "c", "d" }, { "e", "f" } };
		return map.size();
	}
}
