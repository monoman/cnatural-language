public class IntFieldPostIncrement {
	private int field;

	public static bool test() {
		var obj = new IntFieldPostIncrement();
		int i = obj.field++;
		return obj.field == i + 1;
	}
}
