using java.lang;

public class IndexerKeys {
	private String field = "a";
	
	public String this[String s, double d] {
		get {
			return field + s + d;
		}
		set {
			field = value + s + d;
		}
	}
	
	public String method() {
		return this["b", 2] += "c";
	}
	
	public static String test() {
		var obj = new IndexerKeys();
		return obj.method();
	}
}