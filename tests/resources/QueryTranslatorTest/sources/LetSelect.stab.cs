using java.lang;
using stab.query;

public class C {
	public static void m(Iterable<String> s) {
		var query = from e in s
					let x = e.Length
					select e + x;
	}
}