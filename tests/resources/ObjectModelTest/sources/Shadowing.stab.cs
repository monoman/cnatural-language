public class Shadowing {
	public static int test() {
		var obj = new Shadowing();
		return obj.method(2);
	}

	private int x = 1;
	
	public int method(int x) {
		return x + this.x;
	}
}