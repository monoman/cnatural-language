public class NestedClassModification {
	private int field = 2;
	
	public static int test() {
		return new Nested().test();
	}

	public class Nested {
		public int test() {
			NestedClassModification obj = new NestedClassModification();
			obj.field = 3;
			return obj.field;
		}
	}
}