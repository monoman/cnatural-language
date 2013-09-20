using java.lang;
using stab.query;
using stab.tree;

public class ExpressionTreeIndexer {
    public static int test() {
        var t = new int[] { 1, 2 };
        ExpressionTree<FunctionIntInt> expr = p => t[1];
        return (int)((ValueExpression)((BinaryExpression)((ExpressionStatement)expr.Body).Expression).Right).Value;
    }
}
