using java.lang;
using java.util;

public class InferenceExact3 {
	public static int test() {
		return test(new ArrayList<String>());
	}
	
	public static int test<T>(ArrayList<T> t) {
		return 3;
	}
}