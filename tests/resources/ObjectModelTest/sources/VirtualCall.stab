public class VirtualCall : VirtualCallAux {

	public bool method() {
		return super.test1() != ((VirtualCallAux)this).test1();
	}

	public override int test1() {
		return 3;
	}
	
	public static bool test() {
		var obj = new VirtualCall();
		return obj.method();
	}
}

public class VirtualCallAux {
	public virtual int test1() {
		return 2;
	}
}