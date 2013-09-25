using java.lang;

public class StringAdd3 {
	public static String test(String s) {
		return "a" + method("b" + s) + "d";
	}
	
	private static String method(String s) {
		return s;
	}
}
