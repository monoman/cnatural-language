using java.lang;
using stab.query;

public class C {
    public static void m(Iterable<Iterable<String>> s) {
        var query = s.selectMany(e1 => e1, (e1, e2) => new { e1, e2 }).where(query$id0 => query$id0.e2.Length > 0).select(query$id0 => query$id0.e2);
    }
}
