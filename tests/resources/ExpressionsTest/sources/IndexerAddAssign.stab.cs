public class IndexerAddAssign {
	private int field = 2;

	public int this[int idx] {
		get {
			return field;
		}
		set {
			field = value;
		}
	}
	
	public int method() {
		return this[0] += 3;
	}
	
	public static int test() {
		var obj = new IndexerAddAssign();
		return obj.method();
	}
}
