public class DelegateField {
    public static int test() {
        return field();
    }
    
    delegate int D();
    
    private static D field = method;
    
    private static int method() {
        return 2;
    }
}
