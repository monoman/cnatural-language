using java.lang;
using java.lang.annotation;
using java.lang.reflect;

public class ParameterAnnotations {
	public static bool test() {
		Class<?> c = typeof(ParameterAnnotations);
		Method m = c.getMethod("method", typeof(int));
		return m.getParameterAnnotations()[0][1].annotationType() == typeof(ParameterAnnotationsAux);
	}
    
    public static void method([Deprecated][ParameterAnnotationsAux] int arg) {
    }
}

[Target({ ElementType.FIELD, ElementType.PARAMETER })]
[Retention(RetentionPolicy.RUNTIME)]
public interface ParameterAnnotationsAux : Annotation {
}
