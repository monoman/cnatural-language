using java.lang;

public class GenericParameterToString {
	public static string test() {
		var obj = new GenericParameterToStringAux<string>();
		return obj.test("STR");
	}
}

public class GenericParameterToStringAux<T> {
	public string test(T t) {
		return t.toString();
	}
}