using java.lang;
using java.util;
using stab.query;

public class LambdaEnvironment {
	public int method(string s) {
		var l = new ArrayList<string> { "aaa", "bbbb", "ccc" };
		int n = s.length();
		return l.where(p => p.length() == n && p[0] == s[0]).count();
	}
	
	public static int test(string s) {
		var obj = new LambdaEnvironment();
		return obj.method(s);
	}
}
