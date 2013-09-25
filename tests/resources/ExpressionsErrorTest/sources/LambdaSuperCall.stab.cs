public class LambdaSuperCall : LambdaSuperCallAux {
    public static int test() {
        var obj = new LambdaSuperCall();
        return obj.instanceTest();
    }

    delegate int D();
    
    public int instanceTest() {
        D d = () => super.method();
        return d();
    }
    
    public override int method() {
        return 3;
    }
}

public class LambdaSuperCallAux {
    public virtual int method() {
        return 2;
    }
}