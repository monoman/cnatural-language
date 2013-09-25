using java.lang;

public class LambdaAsArgument2 {
    public static int test() {
        run(() => { field++; });
        return field;
    }
    
    private static int field = 1;
    
    private static void run(Runnable r) {
        r.run();
    }
}
