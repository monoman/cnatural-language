using java.lang;

public class GenericParameterToString {
	public static String test() {
		var obj = new GenericParameterToStringAux<String>();
		return obj.test("STR");
	}
}

public class GenericParameterToStringAux<T> {
	public String test(T t) {
		return t.toString();
	}
}