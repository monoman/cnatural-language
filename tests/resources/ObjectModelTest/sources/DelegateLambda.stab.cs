public delegate int DelegateLambdaAux();

public class DelegateLambda {
	public static int test() {
		DelegateLambdaAux d = () => 2;
		return d();
	}	
}
