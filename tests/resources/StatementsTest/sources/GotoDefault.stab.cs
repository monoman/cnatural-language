public class GotoDefault {
	public static int test(int i) {
		int result = 0;
		switch (i) {
		case 1:
			result += 2;
			goto default;
		default:
			result += 3;
			break;
		}
		return result;
	}
}