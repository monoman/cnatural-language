using stab.query;

public class Range {
	public static int test() {
		int result = 1;
		foreach (var i in Query.range(2, 9)) {
			result *= i;
		}
		return result;
	}
}
