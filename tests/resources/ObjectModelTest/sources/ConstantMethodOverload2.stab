public class ConstantMethodOverload2 {
	public int method(int i) {
		return 2;
	}
	
	public int method(byte b) {
		return 1;
	}
	
	public int call() {
		return method((byte)1);
	}
	
	public static int test() {
		var obj = new ConstantMethodOverload2();
		return obj.call();
	}
}