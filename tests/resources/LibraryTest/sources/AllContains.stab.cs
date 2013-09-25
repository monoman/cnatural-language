using java.lang;
using java.util;
using stab.query;

public class AllContains {
	public static bool test() {
		return new AllContains(new ArrayList<string> { "AA", "ZZ" }).method(new ArrayList<string> { "AA", "ZZ", "AA" });
	}
	
	private Iterable<string> strings;
	
	AllContains(Iterable<string> strings) {
		this.strings = strings;
	}
	
	bool method(Iterable<string> si) {
		return si.all(s => strings.contains(s));
	}
}
