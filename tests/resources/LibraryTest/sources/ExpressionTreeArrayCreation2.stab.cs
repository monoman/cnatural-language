using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeArrayCreation2 {
    public static int test() {
        ExpressionTree<FunctionIntT<Object>> expr = p => new int[] { 3 };
        return (int)((ValueExpression)((NewArrayExpression)((ExpressionStatement)expr.Body).Expression).Elements.first()).Value;
    }
}
