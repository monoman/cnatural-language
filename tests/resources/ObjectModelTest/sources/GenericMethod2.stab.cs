using java.lang;

public class GenericMethod2<T> {
	public int field*;

	public void m<U>(T t, U u) {
		field = 3;
	}
	
	public static int test() {
		GenericMethod2<Integer> gm = new GenericMethod2<Integer>();
		gm.m<Integer>(1, 2);
		return gm.field;
	}
}
