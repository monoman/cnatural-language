using java.lang;
using java.util;

public class LocalWildcard {
	public static bool test() {
		Set<?> s = new HashSet<Object>();
		return s.isEmpty();
	}
}
