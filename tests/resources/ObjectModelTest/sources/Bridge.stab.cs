using java.lang;

public class Bridge : Comparable<string> {
	public int compareTo(string s) {
		return s.length();
	}
	
	public static int test() {
		Comparable<string> c = new Bridge();
		return c.compareTo("abc");
	}
}
