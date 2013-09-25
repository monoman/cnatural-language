public class InferenceExact {
	public static int test() {
		return test("");
	}
	
	public static int test<T>(T t) {
		return 3;
	}
}