using java.lang;
using java.util;

public class AutoboxingPuzzle {
	public static int test() {
		Set<Short> s = new HashSet<Short>();
		for (short i = 0; i < 100; i++) {
			s.add(i);
			s.remove(i - 1);
		}
		return s.size();
	}
}
