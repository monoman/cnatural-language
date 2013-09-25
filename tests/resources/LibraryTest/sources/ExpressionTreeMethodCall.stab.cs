using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeMethodCall {
    public static String test() {
        ExpressionTree<FunctionIntInt> expr = p => method(p);
        return ((MethodCallExpression)((ExpressionStatement)expr.Body).Expression).Method.getName();
    }
    
    public static int method(int i) {
        return i;
    }
}
