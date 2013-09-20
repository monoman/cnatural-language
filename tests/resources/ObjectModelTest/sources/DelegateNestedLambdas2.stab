public class DelegateNestedLambdas2 {

	public delegate int D();

	public static int test(int arg) {
		D d1 = () => {
			D d2 = () => arg + 1;
			return d2() + 1;
		};
		return d1();
	}	
}
