using java.lang;
using stab.query;

public class C {
    public static void m(Iterable<String> s1, Iterable<String> s2) {
        var query = s1.join(s2, e1 => e1.Length, e2 => e2.Length, (e1, e2) => e1 + e2);
    }
}
