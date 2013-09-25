using java.lang;

public class AutoboxingFieldIncrement2 {
	private static byte field = 1;

	public static byte test() {
		field += Byte.valueOf(2);
		return field;
	}
}
