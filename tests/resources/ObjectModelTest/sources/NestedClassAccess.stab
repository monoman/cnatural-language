public class NestedClassAccess {
	private int field = 2;
	
	public static int test() {
		return new Nested().getNfield();
	}

	public class Nested {
		private int nfield;
		
		public Nested() {
			nfield = new NestedClassAccess().field;
		}
		
		public int getNfield() {
			return nfield;
		}
	}
}