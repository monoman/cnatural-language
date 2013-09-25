using java.lang;

public class LogicalAnd2 {
	public static int test() {
		var t = new Object[] { new Object(), new Object() };
		int i = 0;
		while (i < sizeof(t) && t[i] != null) {
			i++;
		}
		return i;
	}
}
