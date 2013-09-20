using java.lang;
using java.util;
using stab.query;

public class Where2LINQ {
	public static int test() {
		var query = from s in Query.range(0, 10)
					where s % 2 == 0
					select s;
		return query.count();
	}
}
