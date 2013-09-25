public class NestedClassAccess2 : NestedClassAccess2Aux {
	public static int test() {
		return new Nested().getNfield();
	}

	public class Nested {
		private int nfield;
		
		public Nested() {
			nfield = new NestedClassAccess2().field;
		}
		
		public int getNfield() {
			return nfield;
		}
	}
}

public class NestedClassAccess2Aux {
	protected int field = 2;
}