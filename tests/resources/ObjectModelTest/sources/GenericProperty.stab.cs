using java.lang;

public class GenericProperty<T> {
	public T Property {
		get;
		set;
	}
	
	public static int test() {
		GenericProperty<Integer> gp = new GenericProperty<Integer>();
		gp.Property = 1;
		return gp.Property;
	}
}
