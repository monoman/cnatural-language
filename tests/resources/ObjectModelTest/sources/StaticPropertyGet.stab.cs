public class StaticPropertyGet {
	public static int test() {
		return StaticPropertyGet.Value;
	}

	public static int Value {
		get {
			return 1;
		}
	}
}
