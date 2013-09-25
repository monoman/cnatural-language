using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeAnonymousObjectCreation {
    public static String test() {
        ExpressionTree<FunctionIntT<Object>> expr = p => new { P = "p" };
        return (String)((ValueExpression)((NewObjectExpression)((ExpressionStatement)expr.Body).Expression).Arguments.first()).Value;
    }
}
