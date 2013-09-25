using java.lang;
using java.util;

public class GenericConstraintAssignment2 {
	public static int test() {
		var list = new ArrayList<Integer> { 1, 2 };
		return method(list);
	}
	
	static int method<T>(List<T> list) where T : Number {
		Number n = list[0];
		return n.intValue() + list[1].intValue();
	}
}
