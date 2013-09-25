public class DelegateMethod {
	delegate bool D();

	public bool method() {
		D d = method;
		return d.Method.equals(this.getClass().getMethod("method"));
	}
	
	public static bool test() {
		var obj = new DelegateMethod();
		return obj.method();
	}
}
