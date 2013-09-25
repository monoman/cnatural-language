
public delegate int DelegateAux();

public class Delegate {
	public static int test() {
		var obj = new Delegate();
		return obj.method();
	}

	public int method() {
		DelegateAux d = new DelegateAux(m);
		return d();
	}
	
	public int m() {
		return 2;
	}
}
