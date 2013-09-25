public class InterfaceImplementationAux {
	public int method(int i) {
		return i;
	}
}

public interface InterfaceImplementationAux2 {
	int method(int i);
}

public class InterfaceImplementation : InterfaceImplementationAux, InterfaceImplementationAux2 {
	public static int test() {
		InterfaceImplementationAux2 i = new InterfaceImplementation();
		return i.method(2);
	}
}