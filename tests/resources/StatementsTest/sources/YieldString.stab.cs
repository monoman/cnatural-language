using java.lang;

public class YieldString {
	private string[] t = { "a", "b", "c" };

	public static string test() {
		var obj = new YieldString();
		return obj.method();
	}
	
	public string method() {
		var sb = new StringBuilder();
		var it = strings().iterator();
		while (it.hasNext()) {
			sb.append(it.next());
		}
		return sb.toString();		
	}
	
	public Iterable<string> strings() {
		yield return t[0];
		yield return t[1];
		yield return t[2];
	}
}
