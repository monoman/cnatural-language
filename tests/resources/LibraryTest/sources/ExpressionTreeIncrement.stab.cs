using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeIncrement {
    public static String test() {
        ExpressionTree<FunctionIntInt> expr = p => ++p;
        return ((UnaryExpression)((ExpressionStatement)expr.Body).Expression).Operator.toString();
    }
}
