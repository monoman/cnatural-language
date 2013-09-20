public class GenericBase : GenericBaseAux<GenericBase> {
	public int method() {
		return test1() + test2(this);
	}
	
	public static int test() {
		var obj = new GenericBase();
		return obj.method();
	}
}

public class GenericBaseAux<T> {
	public int test1() {
		return 1;
	}
	
	public int test2(T t) {
		return 2;
	}
}
