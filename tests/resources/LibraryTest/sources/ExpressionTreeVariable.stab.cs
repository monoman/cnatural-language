using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeVariable {
    public static int test() {
        int i = 1;
        ExpressionTree<FunctionIntInt> expr = p => i;
        return (int)((ValueExpression)((ExpressionStatement)expr.Body).Expression).Value;
    }
}
