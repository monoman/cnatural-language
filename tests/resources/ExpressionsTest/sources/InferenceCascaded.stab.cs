using java.lang;

public class InferenceCascaded {
	delegate U D<T, U>(T t);
	
	public static Integer test() {
		return test("STR", p => Integer.valueOf(p.hashCode()));
	}
	
	public static Y test<X, Y>(X x, D<X, Y> d) {
		return d(x);
	}
}
