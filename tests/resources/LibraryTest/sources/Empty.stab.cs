using java.lang;
using stab.query;

public class Empty {
	public static bool test() {
		return !Query.empty<string>().any();
	}
}
