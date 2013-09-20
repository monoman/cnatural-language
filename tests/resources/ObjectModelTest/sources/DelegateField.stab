
public delegate int DelegateFieldAux();

public class DelegateField {
	private DelegateFieldAux field;

	public static int test() {
		var obj = new DelegateField();
		return obj.method();
	}
	
	public int method() {
		this.field = new DelegateFieldAux(m);
		return this.field();
	}
	
	public int m() {
		return 2;
	}
}
