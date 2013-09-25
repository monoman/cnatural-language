public class AutomaticProperty {
	public static int test() {
		var obj = new AutomaticProperty();
		obj.Value = 1;
		return obj.Value;
	}

	public int Value {
		get;
		set;
	}
}
