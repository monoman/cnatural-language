using java.lang;

public class InferenceCascaded3 {
	interface I<T, U> {
		U m(T t);
	}
	
	public Integer method() {
		return test("STR", p => Integer.valueOf(p.hashCode()));
	}
	
	private Y test<X, Y>(X x, I<X, Y> i) {
		return i.m(x);
	}
	
	public static Integer test() {
		var obj = new InferenceCascaded3();
		return obj.method();
	}
}
