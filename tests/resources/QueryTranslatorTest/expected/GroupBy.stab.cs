using java.lang;
using stab.query;

public class C {
    public static void m(Iterable<String> s) {
        var query = s.groupBy(e => e.Length);
    }
}
