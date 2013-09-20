public class DelegateAddArray {
	delegate void D();

	private int field;

	private D[] d = new D[3];

	public int method() {
		d[0] = new D(test1);
		d[1] = test2;
		d[2] = d[0] + d[1];
		d[2]();
		return field;
	}
	
	public void test1() {
		field += 2;
	}
	
	public void test2() {
		field += 3;
	}
	
	public static int test() {
		var obj = new DelegateAddArray();
		return obj.method();
	}
}
