public class NestedInterface : NestedInterfaceAux.Nested {
	public int m() {
		return 1;
	}
	
	public static int test() {
		NestedInterfaceAux.Nested n = new NestedInterface();
		return n.m();
	}
}

public class NestedInterfaceAux {
	public interface Nested {
		int m();
	}
}