public class ConstructorArgument {
	private int field;

	public ConstructorArgument(int i) {
		this.field = i;
	}
	
	public static int test() {
		var obj = new ConstructorArgument(1);
		return obj.field;
	}
}