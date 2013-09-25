using java.lang;

public class GenericClass<T> {
	public T field*;
	
	public static string test(string s) {
		GenericClass<string> gc = new GenericClass<string>();
		gc.field = s;
		return gc.field;
	}
}