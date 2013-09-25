using java.lang;

public class GenericMethod {
	public static string test(string s) {
		return GenericMethod.method<string>(s);
	}
	
	public static T method<T>(T t) {
		return t;
	}
}