using java.lang;
using stab.query;

public class C {
	public static void m(Iterable<string> s) {
		var query = from e in s
					select e into f
					select f;
	}
}