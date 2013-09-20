public class DelegateAddField {
	delegate void D();

	private int field;

	private D d1;
	private D d2;
	private D d3;

	public int method() {
		d1 = new D(test1);
		d2 = new D(test2);
		d3 = d1 + d2;
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
		var obj = new DelegateAddField();
		return obj.method();
	}
}
