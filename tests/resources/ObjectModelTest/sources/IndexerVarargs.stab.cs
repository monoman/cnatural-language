public class IndexerVarargs {
    public static int test() {
        var obj = new IndexerVarargs();
        return obj[1, 2, 3];
    }
    
    public int this[params int[] indexes] {
        get {
            var result = 0;
            foreach (var i in indexes) {
                result += i;
            }
            return result;
        }
    }
}
