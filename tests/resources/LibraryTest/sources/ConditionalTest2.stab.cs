#undef DEBUG

public class ConditionalTest2 {
    public static bool test() {
        try {
            ConditionalAux.assert(false);
        } catch {
            return false;
        }
        return true;
    }
}
