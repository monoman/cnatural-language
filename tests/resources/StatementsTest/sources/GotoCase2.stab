public class GotoCase2 {
	public static int test(int i) {
		int result = 0;
		switch (i) {
		case 2:
			goto case 3;
		default:
			result += 1;
			goto case 2;
		case 3:
			result += 3;
			break;
		}
		return result;
	}
}
