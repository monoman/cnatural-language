using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeField {
    public static string test() {
        ExpressionTree<FunctionIntInt> expr = p => field;
        return ((FieldExpression)((ExpressionStatement)expr.Body).Expression).Field.getName();
    }
    
    static int field = 1;
}
