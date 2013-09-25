public class ThisFieldAccess {
	public static int test() {
		var obj = new ThisFieldAccess();
		return obj.method();
	}
	
	private int field = 1;
	
	private int method() {
		return this.field;
	}
}
