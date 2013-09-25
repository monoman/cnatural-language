public class AnonymousObject3 {
	public static bool test() {
		var obj = new { Name = "Bob", Age = 20 };
		var obj2 = new { obj.Name, obj.Age };
		return obj.equals(obj2);
	}
}