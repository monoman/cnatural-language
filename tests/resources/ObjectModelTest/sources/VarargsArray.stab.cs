using java.lang;

public class VarargsArray {
	public static string method(params string[] values) {
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < sizeof(values); i++) {
			sb.append(values[i]);
		}
		return sb.toString();
	}
	
	public static string test() {
		return method(new string[] { "ab", "c" });
	}
}
