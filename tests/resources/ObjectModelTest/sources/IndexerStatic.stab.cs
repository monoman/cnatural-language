public class IndexerStatic {
	private static int field = 2;
	public static int this[int idx] {
		get {
			return field + idx;
		}
	}
	
	public static int test() {
		return IndexerStatic[3];
	}
}