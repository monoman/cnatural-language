using java.lang;

public class LambdaCatchVariable {
	interface Func { String call(); }

	public static String test() {
		try {
			throw new Exception("Message");
		} catch (Exception e) {
			Func f = () => e.getMessage();
			return f.call();
		}
	}
}
