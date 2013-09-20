using java.lang;
using stab.query;

public class C {
	public static void m(Iterable<String> s1, Iterable<String> s2) {
		var query = from e1 in s1
					join e2 in s2 on e1.Length equals e2.Length into g
					where g.Count > 0
					select e1 + g.first();
	}
}