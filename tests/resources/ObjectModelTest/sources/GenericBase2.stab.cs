using java.lang;

public class GenericBase2 {
	public static int test() {
		GenericBase2Aux2<String, Object> gb = new GenericBase2Aux2<String, Object>();
		return gb.test1(null) + gb.test2("", null);
	}
}

public class GenericBase2Aux<T> {
	public int test1(T t) {
		return 1;
	}
}

public class GenericBase2Aux2<T, U> : GenericBase2Aux<U> {
	public int test2(T t, U u) {
		return 2;
	}
}
