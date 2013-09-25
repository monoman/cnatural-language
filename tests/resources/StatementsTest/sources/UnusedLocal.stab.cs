public class UnusedLocal {
	public static bool test() {
		#pragma warning disable 281
		UnusedLocal l;
		return true;
	}
}
