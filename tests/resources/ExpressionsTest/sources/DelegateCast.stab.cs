using java.lang;

public class DelegateCast {
	delegate int D(int i);
	
	public static int test() {
		Object obj = (D)(p => p + 1);
		return ((D)obj)(1);
	}
}
