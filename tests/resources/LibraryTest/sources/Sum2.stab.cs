using java.lang;
using stab.query;

public class Sum2 {
	public static int test() {
		return Query.asIterable(new[] { "1", "2", "3", "4" }).sum(p => Integer.parseInt(p));
	}
}
