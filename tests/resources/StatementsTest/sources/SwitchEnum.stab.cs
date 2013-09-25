public enum SwitchEnumAux {
	A,
	B,
	C
}

public class SwitchEnum {
	public static int test() {
		int result;
		switch(SwitchEnumAux.B) {
		case A:
			result = 1;
			break;
		case B:
			result = 2;
			break;
		case C:
			result = 3;
			break;
		default:
			result = 4;
			break;
		}
		return result;
	}
}