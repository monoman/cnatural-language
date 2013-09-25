public class StaticShortField {
	private static short field = 1;
	
	public static short test() {
		return StaticShortField.field;
	}
}
