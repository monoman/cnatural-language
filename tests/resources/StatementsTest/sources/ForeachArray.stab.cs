using java.lang;

public class ForeachArray {
	public static String test() {
		String[] t = { "a", "b", "c" };
		
		var sb = new StringBuilder();
		foreach (var s in t) {
			sb.append(s);
		}
		return sb.toString();
	}
}
