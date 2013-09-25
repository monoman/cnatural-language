using java.lang;
using java.util;
using stab.query;

public class Where2 {
	public static int test() {
		return Query.range(0, 10).where(p => p % 2 == 0).count();
	}
}
