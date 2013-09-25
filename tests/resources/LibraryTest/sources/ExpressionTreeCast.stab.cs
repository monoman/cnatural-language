using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeCast {
    public static String test() {
        ExpressionTree<FunctionIntInt> expr = p => (int)p;
        return ((UnaryExpression)((ExpressionStatement)expr.Body).Expression).Operator.toString();
    }
}
