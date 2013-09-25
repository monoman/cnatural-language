using java.lang;
using java.lang.reflect;

public class ParameterAnnotationConstructor {
	public static bool test() {
		Class<?> c = typeof(ParameterAnnotationConstructor);
		var m = c.getConstructor(typeof(int));
		return m.getParameterAnnotations()[0][0] != null;
	}
    
    public ParameterAnnotationConstructor([Deprecated] int arg) {
    }
}
