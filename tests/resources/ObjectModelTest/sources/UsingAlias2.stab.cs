using T1 = p1.UsingAlias2Aux;

namespace p2 {
	public class UsingAlias2 : T1 {
		public static int test() {
			return method();
		}
	}
}

namespace p1 {
	public class UsingAlias2Aux {
		public static int method() {
			return 2;
		}
	}
}
