public class LongFieldPostIncrement {
	private long field;

	public static bool test() {
		var obj = new LongFieldPostIncrement();
		long i = obj.field++;
		return obj.field == i + 1L;
	}
}
