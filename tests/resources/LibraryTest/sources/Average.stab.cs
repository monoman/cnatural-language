using stab.query;

public class Average {
	public static double test() {
		return Query.asIterable(new[] { 0f, 1f, 2f, 3f, 4f }).average();
	}
}
