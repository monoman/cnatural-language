public class AnonymousObjectDelegate {
    public static int test() {
        var obj = new { Delegate = new D(method) };
        return obj.Delegate();
    }
    
    delegate int D();
    
    private static int method() {
        return 2;
    }
}
