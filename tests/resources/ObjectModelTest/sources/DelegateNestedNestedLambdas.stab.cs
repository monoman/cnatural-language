public class DelegateNestedNestedLambdas {

	public delegate int D();

	public static int test(int arg) {
		var l = 2;
		D d1 = () => {
			D d2 = () => {
				D d3 = () => arg + l;
				return arg + d3();
			};
			return l + d2() + 1;
		};
		return d1();
	}	
}
