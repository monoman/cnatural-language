public class ConditionalIf {
	public static bool test(bool c, bool r1, bool r2) {
		if ((c) ? r1 : r2) {
			return true;
		} else {
			return false;
		}
	}
}
