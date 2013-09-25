using java.lang;
using java.util;
using stab.query;

public class LambdaEnvironment {
	public int method(String s) {
		var l = new ArrayList<String> { "aaa", "bbbb", "ccc" };
		int n = s.length();
		return l.where(p => p.length() == n && p[0] == s[0]).count();
	}
	
	public static int test(String s) {
		var obj = new LambdaEnvironment();
		return obj.method(s);
	}
}
