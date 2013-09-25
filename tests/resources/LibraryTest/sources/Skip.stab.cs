using java.lang;
using stab.query;

public class Skip {
	public static string test() {
		return Query.asIterable("ABCDEF").skip(3).aggregate("", (s, c) => s + c);
	}
}
