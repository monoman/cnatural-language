using java.lang;
using stab.query;

public class C {
    public static void m(Iterable<String> s) {
        var query = s.select(e => new { e, x = e.Length }).select(query$id0 => query$id0.e + query$id0.x);
    }
}
