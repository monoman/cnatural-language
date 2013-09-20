public class DelegateLambdaAnonymous {
	delegate int D();

	public static int test() {
		return method(4)();
	}
	
	static D method(int i) {
		var a = new { P = 2 };
		return () => a.P + 1 + i;
	}
}
