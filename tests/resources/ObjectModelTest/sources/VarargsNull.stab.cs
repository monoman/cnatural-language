using java.lang;

public class VarargsNull {
	public static int method(params Object[] args) {
		if (args == null) {
			return 1;
		} else {
			return 2;
		}
	}
	
	public static int test() {
		return method(null) + method((Object)null);
	}
}
