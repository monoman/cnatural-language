public class DelegateAdd2 {
	delegate void D();

	private int field;

	public int method() {
		D d1 = new D(test1);
		D d3 = d1 + test2;
		d3();
		return field;
	}
	
	public void test1() {
		field += 2;
	}
	
	public void test2() {
		field += 3;
	}
	
	public static int test() {
		var obj = new DelegateAdd2();
		return obj.method();
	}
}
