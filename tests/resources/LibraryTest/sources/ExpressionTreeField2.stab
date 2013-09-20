using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeField2 {
    public static String test() {
        var obj = new ExpressionTreeField2();
        ExpressionTree<FunctionIntInt> expr = p => obj.field;
        return ((FieldExpression)((ExpressionStatement)expr.Body).Expression).Field.getName();
    }
    
    int field = 1;
}
