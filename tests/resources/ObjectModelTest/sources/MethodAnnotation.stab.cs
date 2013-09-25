using java.lang;
using java.lang.reflect;

public class MethodAnnotation {
	[Deprecated]
	public static bool test() {
		Class<?> c = typeof(MethodAnnotation);
		Method m = c.getMethod("test");
		return m.getAnnotation(typeof(Deprecated)) != null;
	}
}
