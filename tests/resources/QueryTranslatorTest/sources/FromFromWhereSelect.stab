using java.lang;
using stab.query;

public class C {
	public static void m(Iterable<Iterable<String>> s) {
		var query = from e1 in s
					from e2 in e1
                    where e2.Length > 0
					select e2;
	}
}