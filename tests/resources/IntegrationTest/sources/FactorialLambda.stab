public class FactorialLambda {
	delegate int Func(int n);
	
	public static int test(int n) {
		Func f = null;
		f = p => (p < 1) ? 1 : p * f(p - 1);
		return f(n);
	}
}