using java.lang;

public class WildcardArrayArgument<T> {
	public static bool test() {
		WildcardArrayArgument<?>[] args = new[] {
				new WildcardArrayArgument<string>("str"),
				new WildcardArrayArgument<Integer>(1) };
		var res = method(args);
		return res[0] instanceof string && res[1] instanceof Integer;
	}
	
	T value;
	
	WildcardArrayArgument(T value) {
		this.value = value;
	}
	
	static Object[] method(WildcardArrayArgument<?>[] arg) {
		var t = new Object[2];
		t[0] = arg[0].value;
		t[1] = arg[1].value;
		return t;
	}
}
