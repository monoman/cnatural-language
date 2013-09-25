public class GenericStaticField<T> {
	private static int field = 2;
	
	public static int test() {
		return GenericStaticField.field;
	}
}
