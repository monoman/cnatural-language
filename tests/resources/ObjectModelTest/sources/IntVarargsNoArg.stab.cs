public class IntVarargsNoArg {
	public static int method(params int[] args) {
		int result = 0;
		for (int i = 0; i < sizeof(args); i++) {
			result += args[i];
		}
		return result;
	}
	
	public static int test() {
		return method();
	}
}
