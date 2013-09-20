using java.lang;

public class YieldString {
	private String[] t = { "a", "b", "c" };

	public static String test() {
		var obj = new YieldString();
		return obj.method();
	}
	
	public String method() {
		var sb = new StringBuilder();
		var it = strings().iterator();
		while (it.hasNext()) {
			sb.append(it.next());
		}
		return sb.toString();		
	}
	
	public Iterable<String> strings() {
		yield return t[0];
		yield return t[1];
		yield return t[2];
	}
}
