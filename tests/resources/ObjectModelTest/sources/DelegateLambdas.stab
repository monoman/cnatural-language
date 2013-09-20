public class DelegateLambdas {

	public delegate int D();

	public static int test(int arg) {
		var l = 2;
		D d1 = () => l + 1;
		D d2 = () => arg + 1;
		return d1() + d2();
	}	
}
