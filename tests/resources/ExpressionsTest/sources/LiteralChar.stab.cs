public class LiteralChar {
	public static java.lang.String test() {
		java.lang.StringBuilder sb = new java.lang.StringBuilder();
		sb.append('a');
		sb.append('\u0062');
		sb.append('\x63');
		return sb.toString();
	}
}