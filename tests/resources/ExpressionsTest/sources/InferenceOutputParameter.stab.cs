using java.lang;

public class InferenceOutputParameter {
	public static string test() {
		return method();
	}
	
	public static T method<T>() {
		return null;
	}
}