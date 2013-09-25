using java.lang;

public class VoidLambda {
    private static int field = 1;
    
    public static int test() {
        Runnable r = () => method();
        r.run();
        return field;
    }
    
    private static void method() {
        field++;
    }
}