using java.lang;
using stab.tree;

public class InferenceCascaded4 {
	delegate U D<T, U>(T t);
	
	public static Integer test() {
		return test("STR", p => Integer.valueOf(p.hashCode()));
	}
	
	public static Y test<X, Y>(X x, ExpressionTree<D<X, Y>> t) {
		return null;
	}
}
