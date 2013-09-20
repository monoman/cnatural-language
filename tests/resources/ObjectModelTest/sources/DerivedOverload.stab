public class DerivedOverloadAux {
	public int method(int i) {
		return 1;
	}
}

public class DerivedOverload : DerivedOverloadAux {
	public int method(double d) {
		return 2;
	}
	
	public int call() {
		return method(1);
	}
	
	public static int test() {
		var obj = new DerivedOverload();
		return obj.call();
	}
}