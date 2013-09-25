using java.lang;

public class AutoboxingFieldIncrement {
	private static Byte field = 1;

	public static Byte test() {
		field += 2;
		return field;
	}
}
