using java.lang;

public class GenericArray<T> {
	public static String test(String arg) {
        #pragma warning disable 313
		GenericArray<String> x = new GenericArray<String>();
		x.set(arg);
		return x.get();
	}
	
	private T[] data;
	
	public GenericArray() {
		data = new T[1];
	}
	
	public void set(T t) {
		data[0] = t;
	}
	
	public T get() {
		return data[0];
	}
}
