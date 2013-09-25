using java.lang;
using stab.query;

public class C {
    public static void m(Iterable<String> s1, Iterable<String> s2) {
        var query = s1.groupJoin(s2, e1 => e1.Length, e2 => e2.Length, (e1, g) => new { e1, g }).where(query$id0 => query$id0.g.Count > 0).select(query$id0 => query$id0.e1 + query$id0.g.first());
    }
}
