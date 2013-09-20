public class DelegateAddAssign {
	delegate void D();

	private int field;

	public int method() {
		D d1 = new D(test1);
		D d2 = new D(test2);
		d1 += d2;
		d1();
		return field;
	}
	
	public void test1() {
		field += 2;
	}
	
	public void test2() {
		field += 3;
	}
	
	public static int test() {
		var obj = new DelegateAddAssign();
		return obj.method();
	}
}
