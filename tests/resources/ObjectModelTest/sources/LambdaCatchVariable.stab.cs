using java.lang;

public class LambdaCatchVariable {
	interface Func { string call(); }

	public static string test() {
		try {
			throw new Exception("Message");
		} catch (Exception e) {
			Func f = () => e.getMessage();
			return f.call();
		}
	}
}
