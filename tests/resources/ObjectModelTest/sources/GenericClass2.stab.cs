using java.lang;

public class GenericClass2<T> {
	public T get(T t) {
		return t;
	}
	
	public static string test(string s) {
		return new GenericClass2<string>().get(s);
	}
}