using java.lang;

public class ForeachArray {
	public static string test() {
		string[] t = { "a", "b", "c" };
		
		var sb = new StringBuilder();
		foreach (var s in t) {
			sb.append(s);
		}
		return sb.toString();
	}
}
