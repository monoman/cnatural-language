using java.lang;
using java.io;

public class Using2 {
	public static int test() {
        try {
            using (var obj = new Using2Aux()) {
                obj.fail();
            }
        } catch (Exception) {
        }
        return Using2Aux.closed;
	}
}

public class Using2Aux : Closeable {
    public static int closed;
    public void fail() {
        throw new Exception();
    }
    public void close() {
        closed++;
    }
}