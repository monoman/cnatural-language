using java.lang;
using java.lang.reflect;
using javax.xml.bind.annotation;

public class AnnotationType {
	[XmlElement(type = typeof(Object))]
	public int getTest() {
		return 0;
	}

	public static bool test() {
		Class<?> c = typeof(AnnotationType);
		Method m = c.getMethod("getTest");
		XmlElement elt = m.getAnnotation(typeof(XmlElement));
		return elt.type() == typeof(Object);
	}
}
