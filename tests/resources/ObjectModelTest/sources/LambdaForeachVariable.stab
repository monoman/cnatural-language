using java.lang;
using java.util;

public class LambdaForeachVariable {
	interface Func { String call(); }

	public static String test() {
		var result = "";
		foreach (var s in new ArrayList<String> { "a", "b", "c" }) {
			Func f = () => s;
			result += f.call();
		}
		return result;
	}
}
