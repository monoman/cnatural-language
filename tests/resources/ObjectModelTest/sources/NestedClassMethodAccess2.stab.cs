public class NestedClassMethodAccess2 : NestedClassMethodAccess2Aux {
	public static int test() {
		return new Nested().getNfield();
	}

	public class Nested {
		private int nfield;
		
		public Nested() {
			nfield = new NestedClassMethodAccess2().method();
		}
		
		public int getNfield() {
			return nfield;
		}
	}
}

public class NestedClassMethodAccess2Aux {
	protected int method() {
		return 2;
	}
}