public class IndexerSet {
	public static int test() {
		var obj = new IndexerSet();
		obj[0] = 1;
		return obj.t[0];
	}

	private int[] t = new int[1];
	
	public int this[int idx] {
		set {
			t[idx] = value;
		}
	}
}
