public class DelegateNestedNestedLambdas2 {

	public delegate int D();

	public static int test(int arg) {
		var l = 2;
		D d1 = () => {
			D d2 = () => {
				D d3 = () => arg + l;
				return d3() + 1;
			};
			return d2() + 1;
		};
		return d1();
	}	
}
