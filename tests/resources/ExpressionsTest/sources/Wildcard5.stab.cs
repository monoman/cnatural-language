using java.lang;
using java.util;

public class Wildcard5 {
	public static bool test() {
		var l1 = new ArrayList<string> { "a", "b", "c" };
		var l2 = new ArrayList<string>();
		l2.addAll(l1);
		return l2.size() == 3;
	}
}