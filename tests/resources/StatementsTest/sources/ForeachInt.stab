using java.lang;
using stab.lang;

public class ForeachInt {
	public static String test() {
		var sb = new StringBuilder();
		var first = true;
		foreach (var i in range(0, 5)) {
			if (first) {
				first = false;
			} else {
				sb.append(", ");
			}
			sb.append(i);
		}		
		return sb.toString();
	}
	
	private static IntIterable range(int first, int count) {
		while (count-- > 0) {
			yield return first++;
		}
	}
}
