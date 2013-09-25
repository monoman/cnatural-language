namespace p {
	public class Packages {
		public static int test() {
			return PackagesAux.test() + 1;
		}
	}
}

namespace p {
	public class PackagesAux {
		public static int test() {
			return 2;
		}
	}
}