public interface InterfaceCallAux1 {
	int m1();
}

public interface InterfaceCallAux2 {
	int m2();
}

public class InterfaceCall2 : InterfaceCallAux1, InterfaceCallAux2 {
	public int m1() {
		return 1;
	}
	
	public int m2() {
		return 2;
	}
	
	public static int test() {
		InterfaceCallAux1 i1 = new InterfaceCall2();
		InterfaceCallAux2 i2 = new InterfaceCall2();
		return i1.m1() + i2.m2();
	}
}
