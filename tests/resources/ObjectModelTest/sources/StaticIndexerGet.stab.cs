public class StaticIndexerGet {
	public static int test() {
		return StaticIndexerGet[1];
	}

	public static int this[int idx] {
		get {
			return idx;
		}
	}
}
