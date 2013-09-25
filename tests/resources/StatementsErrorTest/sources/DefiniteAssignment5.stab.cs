public class C {
	public int m(int x, int y) {
		int i;
		if (x >= 0 && (i = y) >= 0) {
			// i definitely assigned
			return i;
		}
		else {
			// i not definitely assigned
			x = y;
		}
		// i not definitely assigned
		return i;
	}
}