using java.lang;
using java.util;

public class LambdaForeachVariable {
	interface Func { string call(); }

	public static string test() {
		var result = "";
		foreach (var s in new ArrayList<string> { "a", "b", "c" }) {
			Func f = () => s;
			result += f.call();
		}
		return result;
	}
}
