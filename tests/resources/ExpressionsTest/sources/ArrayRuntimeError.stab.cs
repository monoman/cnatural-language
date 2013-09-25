using java.lang;

public class ArrayRuntimeError {
	public static bool test() {
		string[] t1 = new string[1];
		Object[] t2 = t1;
		try {
			t2[0] = new Object();
		} catch (ArrayStoreException) {
			return true;
		}
		return false;
	}
}
