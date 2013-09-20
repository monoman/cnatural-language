using java.lang;

public class InferenceCascaded2 {
	interface I<T, U> {
		U m(T t);
	}
	
	public static Integer test() {
		return test("STR", p => Integer.valueOf(p.hashCode()));
	}
	
	public static Y test<X, Y>(X x, I<X, Y> i) {
		return i.m(x);
	}
}
