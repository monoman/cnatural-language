using java.lang;

public class DelegateDynamicInvoke {
	delegate int D();

	public int method() {
		D d = new D(test1);
		return (Integer)d.dynamicInvoke();
	}
	
	public int test1() {
		return 2;
	}
	
	public static int test() {
		var obj = new DelegateDynamicInvoke();
		return obj.method();
	}
}
