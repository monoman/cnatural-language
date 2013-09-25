public class NestedClassOverload {

	private static int method() {
		return 1;
	}

	public static int test() {
		return Nested.test2();
	}

	public class Nested {
		private static int method() {
			return 2;
		}
	
		public static int test2() {
			return method();
		}
	}
}
