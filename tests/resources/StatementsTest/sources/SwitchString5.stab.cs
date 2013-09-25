using java.lang;

public class SwitchString5 {
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
		case "STR3":
			result += 4;
			break;
		case "STR4":
			result += 5;
			break;
		case "STR5":
		case "STR6":
			result += 7;
			break;
		case "STR7":
			result += 8;
			break;
		}
		return result;
	}
}
