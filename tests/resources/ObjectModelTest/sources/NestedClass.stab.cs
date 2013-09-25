public class NestedClass {
	public static int test() {
		return Nested.field;
	}
	
	public class Nested {
		public static int field = 2;
	}
}
