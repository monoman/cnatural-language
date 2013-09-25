using java.lang;
using java.util;

public class ForeachList {
	public static String test() {
		var list = new ArrayList<String>();
		list.add("a");
		list.add("b");
		list.add("c");
		
		var sb = new StringBuilder();
		foreach (var s in list) {
			sb.append(s);
		}
		
		return sb.toString();
	}
}
