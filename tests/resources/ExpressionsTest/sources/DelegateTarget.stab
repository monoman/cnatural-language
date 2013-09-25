public class DelegateTarget {
	delegate bool D();

	public bool method() {
		D d = method;
		return d.Target == this;
	}
	
	public static bool test() {
		var obj = new DelegateTarget();
		return obj.method();
	}
}
