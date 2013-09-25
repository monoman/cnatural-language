public class MethodOverload {
	public int method(int i) {
		return 2;
	}
	
	public int method(byte b) {
		return 1;
	}
	
	public int method() {
		return method((byte)1);
	}
	
	public static int test() {
		var obj = new MethodOverload();
		return obj.method();
	}
}