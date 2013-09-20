public class NestedClassMethodAccess {
	private int method() {
		return 2;
	}
	
	public static int test() {
		return new Nested().getNfield();
	}

	public class Nested {
		private int nfield;
		
		public Nested() {
			nfield = new NestedClassMethodAccess().method();
		}
		
		public int getNfield() {
			return nfield;
		}
	}
}