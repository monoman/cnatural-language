public class Constructor {
	private int field;

	public Constructor() {
		this.field = 1;
	}
	
	public static int test() {
		var obj = new Constructor();
		return obj.field;
	}
}