public class AnonymousObject {
	public static int test() {
		int i = 1;
		var obj = new { i };
		return obj.i;
	}
}