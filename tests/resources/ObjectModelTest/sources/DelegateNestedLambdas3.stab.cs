public class DelegateNestedLambdas3 {

	public delegate int D();

	public static int test(int arg) {
		D d1 = () => {
			D d2 = () => 1;
			return arg + d2() + 1;
		};
		return d1();
	}	
}
