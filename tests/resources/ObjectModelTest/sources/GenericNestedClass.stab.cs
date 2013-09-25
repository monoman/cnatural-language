using java.lang;

public class GenericNestedClass {
	public static string test(string s) {
		return new Nested<string>(s).field;
	}
	
	public class Nested<T> {
		public T field*;
		
		public Nested(T t) {
			field = t;
		}
	}
}
