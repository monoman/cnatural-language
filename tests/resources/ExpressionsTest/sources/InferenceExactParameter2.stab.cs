using java.lang;

public class InferenceExactParameter2 {
	delegate T D<T>(int i);
	
	public static int test() {
		test1((int i) => {
			field += i;
			return "";
		});
		return field;		
	}

	private static int field;
	
	private static void test1<U>(D<U> d) {
		field = 3;
		d(2);
	}
}
