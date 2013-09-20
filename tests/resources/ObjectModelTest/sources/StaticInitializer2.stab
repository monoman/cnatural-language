public class StaticInitializer2 {
	private static int field1 = getMethod1();
	private final static int field2;
	
	static StaticInitializer2() {
		field2 = getMethod2();
	}
	private static int field3 = getMethod3();

	public static int test() {
		return field1 + field2 + field3;
	}
	
	private static int getMethod1() {
		return 1;
	}
	private static int getMethod2() {
		return 2;
	}
	private static int getMethod3() {
		return 3;
	}
}
