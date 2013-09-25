using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeInvoke {
    delegate int D(int i);
    public static String test() {
        D d = method;
        ExpressionTree<FunctionIntInt> expr = p => d(p);
        return ((InvocationExpression)((ExpressionStatement)expr.Body).Expression).Target.Type.getName();
    }
    
    public static int method(int i) {
        return i;
    }
}
