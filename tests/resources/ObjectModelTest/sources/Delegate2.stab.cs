
public delegate int Delegate2Aux();

public class Delegate2 {
	public static int test() {
		var obj = new Delegate2();
		return obj.method();
	}

	public int method() {
		Delegate2Aux d = m;
		return d();
	}
	
	public int m() {
		return 2;
	}
}
