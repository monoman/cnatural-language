public enum SwitchEnum3Aux {
	A,
	B,
	C,
	D,
	E
}

public class SwitchEnum3 {
	public static int test() {
		int result = 0;
		switch(SwitchEnum3Aux.C) {
		case E:
			result = 5;
			break;
		case B:
			result = 2;
			break;
		case D:
			result = 6;
			break;
		case A:
			result = 1;
			break;
		case C:
			goto case B;
		}
		return result;
	}
}
