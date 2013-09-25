public class ObjectInitializer {
	public int Property {
		get;
		set;
	}
	
	public static int test() {
		var obj = new ObjectInitializer { Property = 3 };
		return obj.Property;
	}
}
