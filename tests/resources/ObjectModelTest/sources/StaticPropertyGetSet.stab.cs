public class StaticPropertyGetSet {
	public static int test() {
		StaticPropertyGetSet.Value = 1;
		return StaticPropertyGetSet.Value;
	}

	public static int field;
	
	public static int Value {
		get {
			return field;
		}
		set {
			field = value;
		}
	}
}
