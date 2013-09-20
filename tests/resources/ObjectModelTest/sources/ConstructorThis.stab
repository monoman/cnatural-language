public class ConstructorThis {
	private int field = ConstructorThisAux.method();

	public ConstructorThis() : this(1) {
	}
	
	public ConstructorThis(int i) {
	}
	
	public static int test() {
		new ConstructorThis();
		return ConstructorThisAux.count;
	}
}

public class ConstructorThisAux {
	public static int count = 0;
	
	public static int method() {
		count++;
		return 1;
	}
}
