using java.lang;
using java.util;
using stab.query;

public class SequenceEqual {
	public static bool test() {
		return new ArrayList<string> { "a", "b", "c" }.sequenceEqual(Query.asIterable(new[] { "a", "b", "c" }));
	}
}
