public class NestedClassOverload2 {

	private static int method(int i) {
		return 1;
	}

	public static int test() {
		return Nested.test2();
	}

	public class Nested {
		private static int method(double d) {
			return 2;
		}
	
		public static int test2() {
			return method(1);
		}
	}
}
