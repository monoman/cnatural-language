public class DelegateLambdaSideEffect {

	public delegate int D();

	public static int test() {
		var l = 2;
		D d = () => l++;
		return d() + l;
	}	
}
