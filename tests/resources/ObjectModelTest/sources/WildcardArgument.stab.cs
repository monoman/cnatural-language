using java.lang;

public class WildcardArgument<T> {
	public static bool test() {
		WildcardArgument<string> obj1 = new WildcardArgument<string>("str");
		WildcardArgument<Integer> obj2 = new WildcardArgument<Integer>(1);
		return method(obj1) instanceof string && method(obj2) instanceof Integer;
	}
	
	T value;
	
	WildcardArgument(T value) {
		this.value = value;
	}
	
	static Object method(WildcardArgument<?> arg) {
		return arg.value;
	}
}
