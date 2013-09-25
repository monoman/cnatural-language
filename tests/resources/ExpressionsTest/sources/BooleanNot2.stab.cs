public class BooleanNot2 {
	public static bool test() {
		var b = true;
		return ((b = !b) == false);
	}
}