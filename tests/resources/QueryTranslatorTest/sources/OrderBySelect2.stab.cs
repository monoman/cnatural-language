using java.lang;
using stab.query;

public class C {
	public static void m(Iterable<String> s) {
		var query = from e in s
					orderby e.Length, e descending
					select e;
	}
}