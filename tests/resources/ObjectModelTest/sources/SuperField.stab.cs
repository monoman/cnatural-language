public class SuperField : SuperFieldAux {
	private int field = 1;
	
	public int method() {
		return super.field;
	}
	
	public static int test() {
		var obj = new SuperField();
		return obj.method();
	}
}

public class SuperFieldAux {
	protected int field = 2;	
}
