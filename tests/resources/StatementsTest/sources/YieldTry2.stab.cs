using java.lang;
using stab.lang;

public class YieldTry2 {
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
			if (field == 1) {
				throw new IllegalStateException();
			}
			field++;
		} catch {
			field += 10;
		} finally {
			field++;
		}
	}
}
