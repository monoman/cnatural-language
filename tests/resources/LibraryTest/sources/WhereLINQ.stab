using java.lang;
using java.util;
using stab.query;

public class WhereLINQ {
	public static int test() {
		var list = new ArrayList<String> { "a", "bb", "ccc" };
		var query = from s in list
					where s.length() > 1
					select s;
		return query.count();
	}
}
