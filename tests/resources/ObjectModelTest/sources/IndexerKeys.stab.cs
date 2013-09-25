using java.lang;

public class IndexerKeys {
	private string field = "a";
	
	public string this[string s, double d] {
		get {
			return field + s + d;
		}
		set {
			field = value + s + d;
		}
	}
	
	public string method() {
		return this["b", 2] += "c";
	}
	
	public static string test() {
		var obj = new IndexerKeys();
		return obj.method();
	}
}