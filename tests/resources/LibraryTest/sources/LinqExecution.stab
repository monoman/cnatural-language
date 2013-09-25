using stab.query;

public class LinqExecution {
	public static bool test() {
		var i = 0;
		var query = from n in Query.range(0, 10)
					select ++i;
		foreach (var n in query) {
			if (n != i) {
				return false;
			}
		}
		return true;
	}
}
