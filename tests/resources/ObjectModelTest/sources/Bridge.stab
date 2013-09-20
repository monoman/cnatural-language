using java.lang;

public class Bridge : Comparable<String> {
	public int compareTo(String s) {
		return s.length();
	}
	
	public static int test() {
		Comparable<String> c = new Bridge();
		return c.compareTo("abc");
	}
}
