using java.lang;
using stab.query;

public class C {
	public static void m(Iterable<string> s1, Iterable<string> s2) {
		var query = from e1 in s1
					from e2 in s2
					select e1 + e2;
	}
}