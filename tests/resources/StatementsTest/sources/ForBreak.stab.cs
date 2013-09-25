using java.lang;
using java.util;
using stab.query;

public class ForBreak {
	public static string test() {
		var l = new ArrayList<string> { "str0", "str1" };
		string s;
		if ("str" == null) {
			s = "str0";
		} else {
			int n = 0;
			for (;;) {
				s = "str" + n++;
				if (l.all(p => !p.equals(s))) {
					break;
				}
			}
		}
		return s;
	}
}
