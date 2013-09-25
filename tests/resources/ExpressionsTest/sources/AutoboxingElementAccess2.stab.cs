using java.lang;

public class AutoboxingElementAccess2 {
	public static Integer test(int i) {
		int[] t = { i };
		t[0] += Integer.valueOf(2);
		return t[0];
	}
}