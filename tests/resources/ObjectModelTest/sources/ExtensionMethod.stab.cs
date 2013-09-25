using java.lang;

public class ExtensionMethod {
	public static int test() {
		return "abc".size();		
	}
}

public static class ExtensionMethodAux {
	public static int size(this string s) {
		return s.length();
	}
}
