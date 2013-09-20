using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeObjectCreation3 {
    public static String test() {
        ExpressionTree<FunctionIntT<Object>> expr = p => new ExpressionTreeObjectCreation3 { Property = 1 };
        return ((NewObjectExpression)((ExpressionStatement)expr.Body).Expression).Initializers.first().Member.getName();
    }
    
    int Property {
        get;
        set;
    }
}
