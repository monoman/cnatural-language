using java.lang;

public class Synchronized {
	public static void test() {
		synchronized (new Object()) {
			return;
		}
	}
}
