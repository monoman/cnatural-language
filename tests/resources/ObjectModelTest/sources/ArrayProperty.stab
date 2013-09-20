public class ArrayProperty {
	public int[] Values {
		get;
		set;
	}
	
	public int method() {
		Values = new int[10];
		for (int i = 0; i < sizeof(Values); i++) {
			Values[i] = i;
		}
		return this.Values[9];
	}
	
	public static int test() {
		var obj = new ArrayProperty();
		return obj.method();
	}
}