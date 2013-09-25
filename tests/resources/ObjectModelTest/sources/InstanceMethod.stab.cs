public class InstanceMethod {
	public static int test() {
		var obj = new InstanceMethod();
		return obj.instanceMethod();
	}
	
	private int instanceMethod() {
		return 1;
	}
}
