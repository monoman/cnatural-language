using stab.query;

public class IntList2 {
	public static bool test() {
		var list = Query.range(0, 5).toList();
		list.removeAt(2);
		return list.sequenceEqual(Query.asIterable(new int[] { 0, 1, 3, 4 }));
	}
}
