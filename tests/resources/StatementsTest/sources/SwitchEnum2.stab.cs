public enum SwitchEnum2Aux {
	A,
	B,
	C
}

public class SwitchEnum2 {
	public static int test() {
		int result;
		switch(SwitchEnum2Aux.C) {
		case A:
			result = 1;
			break;
		case B:
			result = 2;
			break;
		case C:
			goto case B;
		default:
			result = 4;
			break;
		}
		return result;
	}
}