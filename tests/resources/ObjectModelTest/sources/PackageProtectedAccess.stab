public class PackageProtectedAccess {
	public static int test() {
		var obj = new PackageProtectedAccessAux();
		return obj.Prop;
	}
}

public class PackageProtectedAccessAux {
	protected int Prop {
		get {
			return 2;
		}
	}
}
