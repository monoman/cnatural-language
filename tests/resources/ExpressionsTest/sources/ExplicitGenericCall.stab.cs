using java.lang;

public class ExplicitGenericCall {
	public static int test() {
		return method<string>();
	}

	public static int method<T>() {
		return 2;
	}
}