using java.lang;

public class MethodOverload2 {
	public static int m1(Object o, int i) {
		return 1;
	}
	
	public static int m1(Object o, Object[] t) {
		return 2;
	}
	
	public static int m2(Object o, int i) {
		return 100;
	}
	
	public static int m2(Object o, Object[] t) {
		return 200;
	}
	
	public static int test() {
		return m1(null, 1) + m2(null, null);
	}
}