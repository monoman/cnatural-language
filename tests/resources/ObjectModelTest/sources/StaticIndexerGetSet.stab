public class StaticIndexerGetSet {
	public static int test() {
		StaticIndexerGetSet[0] = 1;
		return StaticIndexerGetSet[0];
	}

	private static int[] t = new int[1]; 
	
	public static int this[int idx] {
		set {
			t[idx] = value;
		}
		get {
			return t[idx];
		}
	}
}
