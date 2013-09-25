using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeObjectCreation2 {
    public static string test() {
        ExpressionTree<FunctionIntT<Object>> expr = p => new ExpressionTreeObjectCreation2 { field = 1 };
        return ((NewObjectExpression)((ExpressionStatement)expr.Body).Expression).Initializers.first().Member.getName();
    }
    
    int field;
}
