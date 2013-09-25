public class AnonymousObject2 {
	public static int test() {
		var obj = new { Name = "Bob", Age = 20 };
		var obj2 = obj;
		return obj2.Age;
	}
}