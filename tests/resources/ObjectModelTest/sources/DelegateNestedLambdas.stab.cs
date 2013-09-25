public class DelegateNestedLambdas {

	public delegate int D();

	public static int test(int arg) {
		var l = 2;
		D d1 = () => {
			D d2 = () => arg + 1;
			return l + d2() + 1;
		};
		return d1();
	}	
}
