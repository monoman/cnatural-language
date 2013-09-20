using java.lang;

public class ObjectArrayBoxing3 {
	public static double test() {
		var obj = new ObjectArrayBoxing3();
		return (Double)obj.method()[0];
	}
	
	double[] field = new double[] { 3d, 4d };
	
	Object[] method() {
		var o = new Object[2];
		for (int i = 0; i < 2; i++) {
			o[i] = field[i];
		}
		return o;
	}
}
