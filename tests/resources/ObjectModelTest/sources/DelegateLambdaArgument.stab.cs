public class DelegateLambdaArgument {

	public delegate int D(int i);

	public static int test(int i) {
		D d = p => i + p;
		return d(2);
	}	
}
