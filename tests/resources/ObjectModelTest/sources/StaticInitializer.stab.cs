public class StaticInitializer {
	private static int field = getFieldValue();
	
	public static int test() {
		return field;
	}
	
	private static int getFieldValue() {
		return 1;
	}
}