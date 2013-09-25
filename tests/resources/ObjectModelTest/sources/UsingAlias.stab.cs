using java.lang;
using SB = java.lang.StringBuilder;

public class UsingAlias {
	public static string test(string s1, string s2) {
		SB sb = new SB(s1);
		sb.append(s2);
		return sb.toString();
	}
}