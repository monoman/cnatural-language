public class BooleanNot {
	public static bool test() {
		var b = true;
		if ((b = !b) == false) {
			return true;
		}
		return false;
	}
}