using stab.query;

public class IntSet1 {
	public static bool test() {
		var set = Query.asIterable(new[] { 1, 2, 3, 2, 1 }).toSet();
		return !set.except(Query.asIterable(new[] { 1, 2, 3 })).any();
	}
}
