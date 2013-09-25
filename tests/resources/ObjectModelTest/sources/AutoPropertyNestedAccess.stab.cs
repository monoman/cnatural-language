public class AutoPropertyNestedAccess {
	public static int test() {
		var obj = new AutoPropertyNestedAccess();
		return obj.nested.Prop;
	}

	Nested nested;
	
	AutoPropertyNestedAccess() {
		this.nested = new Nested(this);
	}
	
	public int Prop {
		get;
		private set;
	}
	
	private class Nested {
		private AutoPropertyNestedAccess outer;
	
		Nested(AutoPropertyNestedAccess outer) {
			this.outer = outer;
			outer.Prop = 1;
		}
		
		int Prop {
			get {
				return outer.Prop;
			}
		}
	}
}