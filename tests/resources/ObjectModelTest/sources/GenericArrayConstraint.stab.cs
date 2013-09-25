using java.lang;

public class GenericArrayConstraint<T> where T : java.lang.CharSequence {
	public static String test(String arg) {
		GenericArrayConstraint<String> x = new GenericArrayConstraint<String>();
		x.set(arg);
		return x.get();
	}
	
	private T[] data;
	
	public GenericArrayConstraint() {
        #pragma warning disable 313
		data = new T[1];
	}
	
	public void set(T t) {
		data[0] = t;
	}
	
	public T get() {
		return data[0];
	}
}
