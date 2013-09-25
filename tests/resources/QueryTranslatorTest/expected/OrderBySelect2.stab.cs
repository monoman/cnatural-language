using java.lang;
using stab.query;

public class C {
    public static void m(Iterable<String> s) {
        var query = s.orderBy(e => e.Length).thenByDescending(e => e);
    }
}
