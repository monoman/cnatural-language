using java.lang;

public class InferredReturnType {
    public static string test() {
        var t = new string[] { "s1", "s2" };
        var s = method(t, p => { if (sizeof(t) > 1) return p; else return null; });
        return s;
    }
    
    delegate R D<P, R>(P p);
    
    static U method<T, U>(T[] t, D<T, U> d) {
        return d(t[1]);
    }
}
