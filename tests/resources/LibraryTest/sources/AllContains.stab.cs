using java.lang;
using java.util;
using stab.query;

public class AllContains {
	public static bool test() {
		return new AllContains(new ArrayList<String> { "AA", "ZZ" }).method(new ArrayList<String> { "AA", "ZZ", "AA" });
	}
	
	private Iterable<String> strings;
	
	AllContains(Iterable<String> strings) {
		this.strings = strings;
	}
	
	bool method(Iterable<String> si) {
		return si.all(s => strings.contains(s));
	}
}
