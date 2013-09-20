using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeTypeof {
    public static String test() {
        ExpressionTree<FunctionIntT<Class<?>>> expr = p => typeof(ExpressionTreeTypeof);
        return ((Class<?>)((ValueExpression)((ExpressionStatement)expr.Body).Expression).Value).getName();
    }
}
