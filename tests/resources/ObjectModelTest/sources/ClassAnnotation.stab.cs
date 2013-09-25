using java.lang;

[Deprecated]
public class ClassAnnotation {
	public static bool test() {
		Class<?> c = typeof(ClassAnnotation);
		return c.getAnnotation(typeof(Deprecated)) != null;
	}
}
