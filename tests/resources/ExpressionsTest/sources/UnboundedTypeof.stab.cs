using java.lang;
using java.util;

public class UnboundedTypeof {
	public static Class<?> test() {
		return typeof(ArrayList<?>);
	}
}
