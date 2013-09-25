using java.lang;
using stab.lang;

public class ConditionalAux {
    [Conditional({ "DEBUG" })]
    public static void assert(bool test) {
        if (!test) {
            throw new Exception("Assertion Failed");
        }
    }
}
