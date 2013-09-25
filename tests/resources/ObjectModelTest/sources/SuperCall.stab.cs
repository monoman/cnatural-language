public class SuperCall : SuperCallAux {
	public static int test() {
		var obj = new SuperCall();
		return obj.method();
	}

	public override int method() {
		return super.method() + 1;
	}
}

public class SuperCallAux {
	public virtual int method() {
		return 1;
	}
}