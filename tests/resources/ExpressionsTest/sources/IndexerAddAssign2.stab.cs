public class IndexerAddAssign2 {
	private long field = 2;

	public long this[long idx] {
		get {
			return field;
		}
		set {
			field = value;
		}
	}
	
	public long method() {
		return this[0] += 3;
	}
	
	public static long test() {
		var obj = new IndexerAddAssign2();
		return obj.method();
	}
}
