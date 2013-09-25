using java.lang;
using stab.query;

public class Reverse {
	public static string test() {
		return Query.asIterable("ABCDEF").reverse().aggregate("", (s, c) => s + c);
	}
}
