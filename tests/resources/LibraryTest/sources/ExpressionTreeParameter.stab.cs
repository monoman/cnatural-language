using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeParameter {
    public static String test() {
        ExpressionTree<FunctionIntInt> expr = p => p;
        return ((VariableExpression)((ExpressionStatement)expr.Body).Expression).Name;
    }
}