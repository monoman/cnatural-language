public class GotoCase {
	public static int test(int i) {
		int result = 0;
		switch (i) {
		case 2:
			result += 2;
			goto case 3;
		case 1:
			result += 3;
			goto case 2;
		case 3:
			result += 4;
			break;
		}
		return result;
	}
}
