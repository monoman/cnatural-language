using java.lang;
using stab.lang;

public class YieldLoop {
	public static String test() {
		var obj = new YieldLoop();
		return obj.method();
	}

	public String method() {
		var sb = new StringBuilder();
		var it = power(2, 8).iterator();
		var first = true;
		while (it.hasNext()) {
			if (first) {
				first = false;
			} else {
				sb.append(", ");
			}
			sb.append("" + it.nextInt());
		}
		return sb.toString();	
	}
	
	private IntIterable power(int number, int exponent) {
		var counter = 0;
		var result = 1;
		while (counter++ < exponent) {
			result *= number;
			yield return result;
		}
	}
}
