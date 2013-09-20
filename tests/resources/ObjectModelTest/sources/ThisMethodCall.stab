public class ThisMethodCall {
	public static int test() {
		var obj = new ThisMethodCall();
		return obj.method();
	}
	
	private int method() {
		return this.instanceMethod();
	}
	
	private int instanceMethod() {
		return 1;
	}
}