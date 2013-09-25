using java.lang;

public class ArrayRuntimeError {
	public static bool test() {
		String[] t1 = new String[1];
		Object[] t2 = t1;
		try {
			t2[0] = new Object();
		} catch (ArrayStoreException) {
			return true;
		}
		return false;
	}
}
