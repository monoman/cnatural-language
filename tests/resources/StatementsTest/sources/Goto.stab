public class Goto {
	public static int test(int n) {
		start:
		if (n > 0)
			goto pos;
		goto neg;
		pos:
		return n;
		neg:
		n = -n;
		goto start;
	}
}