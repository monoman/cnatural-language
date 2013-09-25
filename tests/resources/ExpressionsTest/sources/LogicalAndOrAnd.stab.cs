public class LogicalAndOrAnd {
	public static bool test(bool a, bool b, bool c, bool d) {
		if ((a && b) || (c && d)) {
			return true;
		} else {
			return false;
		}
	}
}