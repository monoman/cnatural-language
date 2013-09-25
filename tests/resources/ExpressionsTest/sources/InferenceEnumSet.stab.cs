using java.util;

public class InferenceEnumSet {
	public static int test() {
		var set = EnumSet.noneOf(typeof(E));
		return set.size();
	}
	
	private enum E {
		A,
		B
	}
}