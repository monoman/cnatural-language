using java.lang;
using java.io;

public class Using3 {
	public static int test() {
        for (int i = 0; i < 4; i++) {
            #pragma warning disable 281
            using (var obj = new Using3Aux()) {
                if (i % 2 == 0) {
                    continue;
                }
            }
        }
        return Using3Aux.closed;
	}
}

public class Using3Aux : Closeable {
    public static int closed;
    public void close() {
        closed++;
    }
}