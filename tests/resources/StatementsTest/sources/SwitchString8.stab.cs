using java.lang;

public class SwitchString8 {
	public static int test(String s) {
		switch (s) {
		case "STR1":
		case "STR2":
		case "STR3":
		case "STR4":
		default:
		case "STR5":
		case "STR6":
			return 1;
		case "STR7":
			break;
		}
		return 0;
	}
}
