using java.lang;
using SB = java.lang.StringBuilder;

public class UsingAlias {
	public static String test(String s1, String s2) {
		SB sb = new SB(s1);
		sb.append(s2);
		return sb.toString();
	}
}