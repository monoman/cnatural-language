using java.lang;
using java.util;

public class WildcardBoundAssignment {
	public static int test() {
		var list = new ArrayList<Integer> { 1, 2 };
		return method(list);
	}
	
	static int method(List<? : Number> list) {
		Number n = list[0];
		return n.intValue() + 2;
	}
}
