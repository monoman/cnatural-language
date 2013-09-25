using java.lang;
using stab.query;

public class C {
    public static void m(Iterable<string> s) {
        var query = s.groupBy(e => e.Length);
    }
}
