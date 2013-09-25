public class ArrayFieldInitializer {
	private int[] t = { 1, 2, 3 };
	public int method() {
		return t[2];
	}
	
	public static int test() {
		var obj = new ArrayFieldInitializer();
		return obj.method();
	}
}
