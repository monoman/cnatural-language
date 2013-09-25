public class StaticPropertySet {
	public static int test() {
		StaticPropertySet.Value = 1;
		return StaticPropertySet.field;
	}

	public static int field;
	
	public static int Value {
		set {
			field = value;
		}
	}
}
