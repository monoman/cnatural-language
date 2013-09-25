public class NestedIf {
	public static int test(int value) {
		if (value == 0)
			if (value == 1)
				return 1;
			else
				return 2;
		return 3;
	}
}