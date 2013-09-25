using java.lang;

public class ObjectArrayBoxing {
	public static int test() {
		var a = new int[] { 3 };
		var o = new Object[1];
		o[0] = a[0];
		return (Integer)o[0];
	}
}
