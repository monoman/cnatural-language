using java.lang;

public class SwitchString4 {
	public static int test(String s) {
		int result = 0;
		switch (s) {
		case "STR1":
			result += 1;
			break;
		default:
		case "STR2":
			result += 3;
			break;
		}
		return result;
	}
}
