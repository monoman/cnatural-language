using java.lang;

public class ObjectArrayBoxing2 {
	public static double test() {
		var a = new double[] { 3d };
		var o = new Object[1];
		o[0] = a[0];
		return (Double)o[0];
	}
}
