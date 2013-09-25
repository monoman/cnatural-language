using java.lang;
using java.io;

public class Using {
	public static int test() {
        #pragma warning disable 281
        using (var obj = new UsingAux()) {
        }
        return UsingAux.closed;
	}
}

public class UsingAux : Closeable {
    public static int closed;
    public void close() {
        closed++;
    }
}