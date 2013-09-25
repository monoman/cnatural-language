using java.lang;

public class InferenceExact2 {
	public static int test() {
		return test(new string[0]);
	}
	
	public static int test<T>(T[] t) {
		return 3;
	}
}