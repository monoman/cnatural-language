using java.lang;

public class WildcardConstraintArgument<T> {
	public static bool test() {
		WildcardConstraintArgument<string> obj1 = new WildcardConstraintArgument<string>("str");
		WildcardConstraintArgument<Integer> obj2 = new WildcardConstraintArgument<Integer>(1);
		return method(obj1) instanceof Double && method(obj2) instanceof Integer;
	}
	
	T value;
	
	WildcardConstraintArgument(T value) {
		this.value = value;
	}
	
	static Object method(WildcardConstraintArgument<? : Number> arg) {
		return arg.value;
	}
}
