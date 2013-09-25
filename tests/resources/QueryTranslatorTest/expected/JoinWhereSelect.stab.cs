using java.lang;
using stab.query;

public class C {
    public static void m(Iterable<String> s1, Iterable<String> s2) {
        var query = s1.join(s2, e1 => e1.Length, e2 => e2.Length, (e1, e2) => new { e1, e2 }).where(query$id0 => query$id0.e2.Length > 1).select(query$id0 => query$id0.e1 + query$id0.e2);
    }
}
