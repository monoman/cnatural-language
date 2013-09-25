using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeProperty {
    public static String test() {
        ExpressionTree<FunctionIntInt> expr = p => Property;
        return ((MethodCallExpression)((ExpressionStatement)expr.Body).Expression).Method.getName();
    }
    
    public static int Property {
        get {
            return 1;
        }
    }
}
