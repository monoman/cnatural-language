public class NestedClassConstructorAccess {
	private int field;
	private NestedClassConstructorAccess() {
		this.field = 2;
	}
	
	public static int test() {
		return new Nested().getNfield();
	}

	public class Nested {
		private int nfield;
		
		public Nested() {
			nfield = new NestedClassConstructorAccess().field;
		}
		
		public int getNfield() {
			return nfield;
		}
	}
}
