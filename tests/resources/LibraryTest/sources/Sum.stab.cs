using stab.query;

public class Sum {
	public static int test() {
		return Query.asIterable(new[] { 1, 2, 3, 4 }).sum();
	}
}
