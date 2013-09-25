using java.lang;
using stab.query;

public class C {
	public static void m(Iterable<String> s1, Iterable<String> s2, Iterable<String> s3) {
		var query = from e1 in s1
					from e2 in s2
					from e3 in s3
					select e1 + e2 + e3;
	}
}