using java.lang;

public class InferenceExactParameter {
	delegate T D<T>(int i);
	
	public static int test() {
		test1(m);
		return field;		
	}

	private static String m(int i) {
		field += i;
		return "";
	}
	
	private static int field;
	
	private static void test1<U>(D<U> d) {
		field = 3;
		d(2);
	}
}
