public class NestedClassSiblings2 {
	private class Nested1 {
		public static int field = 2;
	}
	
	private class Nested2 {
		public class Nested3 {
			public static int field = Nested1.field;
		}
	}

	public static int test() {
		return Nested2.Nested3.field;
	}
}
