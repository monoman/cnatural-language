using stab.lang;

public class YieldTry {
	public static int test() {
		var it = iter().iterator();
		it.next();
		it.hasNext();
		return field;
	}
	
	private static int field;
	
	private static IntIterable iter() {
		try {
			field++;
			yield return 1;
			field++;
		} finally {
			field++;
		}
	}
}