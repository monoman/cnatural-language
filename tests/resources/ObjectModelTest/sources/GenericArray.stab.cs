using java.lang;

public class GenericArray<T> {
	public static string test(string arg) {
        #pragma warning disable 313
		GenericArray<string> x = new GenericArray<string>();
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
