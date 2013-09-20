public class LambdaInConstructor {
	public static int test() {
		return new LambdaInConstructor().d();
	}
	
	delegate int D();
	int field = 3;
	D d;
	
	LambdaInConstructor() {
		d = () => field;
	}
}
