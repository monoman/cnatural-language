public class IndexerGetSet {
	public static int test() {
		var obj = new IndexerGetSet();
		obj[0] = 1;
		return obj[0];
	}

	private int[] t = new int[1];
	
	public int this[int idx] {
		get {
			return t[idx];
		}
		set {
			t[idx] = value;
		}
	}
}
