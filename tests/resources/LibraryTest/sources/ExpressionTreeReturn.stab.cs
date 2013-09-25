using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeReturn {
    public static int test() {
        ExpressionTree<FunctionIntInt> expr = p => { return 1; };
        return (int)((ValueExpression)((ReturnStatement)((BlockStatement)expr.Body).Statements.first()).Value).Value;
    }
}
