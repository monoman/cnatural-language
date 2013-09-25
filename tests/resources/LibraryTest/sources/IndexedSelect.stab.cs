using java.lang;
using stab.query;

public class IndexedSelect {
	public static string test() {
		var it = Query.triple("A", "B", "C");
		return it.select((p, i) => p + i).aggregate("", (p, q) => p + q);
	}
}
