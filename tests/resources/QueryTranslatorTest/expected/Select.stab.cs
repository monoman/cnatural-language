using java.lang;

public class C {
    public static void m(Iterable<String> s) {
        var query = s.select(e => e);
    }
}
