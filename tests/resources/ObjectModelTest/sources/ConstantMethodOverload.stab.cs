public class ConstantMethodOverload {
	public int method(int i) {
		return 2;
	}
	
	public int method(byte b) {
		return 1;
	}
	
	public int call() {
		return method(1);
	}
	
	public static int test() {
		var obj = new ConstantMethodOverload();
		return obj.call();
	}
}