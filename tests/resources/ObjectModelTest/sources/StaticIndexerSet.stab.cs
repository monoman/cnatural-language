public class StaticIndexerSet {
	public static int test() {
		StaticIndexerSet[0] = 1;
		return t[0];
	}

	public static int[] t = new int[1];
	
	public static int this[int idx] {
		set {
			t[idx] = value;
		}
	}
}
