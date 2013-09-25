using java.lang;

namespace p.q {

	public class PrivateOuterField<U, V> {
		public static int test() {
			var obj = new PrivateOuterField<Integer, string>();
			new Nested(obj);
			return obj.field;
		}

		private int field;

		class Nested {
			Nested(PrivateOuterField<Integer, string> obj) {
				obj.field = 2;
			}
		}
	}

}
