using java.lang;
using stab.query;

public class IntIterable2 {
	public static bool test() {
		Iterable<Integer> it1 = Query.range(0, 5).toList();
		Iterable<Integer> it2 = Query.range(0, 5);
		return it1.sequenceEqual(it2);
	}
}
