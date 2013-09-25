using java.lang;
using stab.query;

public class C {
    public static void m(Iterable<string> s1, Iterable<string> s2) {
        var query = s1.selectMany(e1 => s2, (e1, e2) => e1 + e2);
    }
}
