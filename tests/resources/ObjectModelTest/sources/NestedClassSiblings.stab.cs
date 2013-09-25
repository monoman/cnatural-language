public class NestedClassSiblings {
	private class Nested1 {
		public static int field = 2;
	}
	
	private class Nested2 {
		public static int field = Nested1.field;
	}

	public static int test() {
		return Nested2.field;
	}
}
