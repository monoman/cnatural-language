using java.lang;

public class C {
	public static void m(Iterable<String> s) {
		var query = from e in s
					select e;
	}
}