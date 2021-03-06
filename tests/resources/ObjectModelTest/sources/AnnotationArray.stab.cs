using java.lang;
using java.lang.reflect;
using javax.xml.bind.annotation;

[XmlSeeAlso(value = { typeof(Object), typeof(string) })]
public class AnnotationArray {

	public static bool test() {
		var c = typeof(AnnotationArray);
		var a = c.getAnnotation(typeof(XmlSeeAlso));
		return a.value()[0] == typeof(Object);
	}
}
