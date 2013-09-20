public class ObjectArray {
	private int value;
	
	public ObjectArray(int value) {
		this.value = value;
	}
	
	static ObjectArray create(int i) {
		return new ObjectArray(i);
	}
	
	public static int test() {
		ObjectArray[] t = { new ObjectArray(1), create(2) };
		return t[0].value + t[1].value;
	}
}