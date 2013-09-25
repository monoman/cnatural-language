public class DelegateLambdaField {
	public static int test() {
		var obj = new DelegateLambdaField();
		return obj.method();
	}

	public delegate int D(int i);

	private int field = 2;

	public int method() {
		D d = p => field + p;
		return d(3);
	}	
}
