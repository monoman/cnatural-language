using java.lang;

public class TryFinallyLocal {
	static int field;

	public static int test() {
		int result = 0;
		try {
			if (field == 0) {
				result = 1;
			} else {
				result = 2;
			}
		} catch (Exception e) {
			method2(e);
		} finally {
			method();
		}
		return result;
	}
	
	static void method() {
	}
	
	static void method2(Object obj) {
	}
}
