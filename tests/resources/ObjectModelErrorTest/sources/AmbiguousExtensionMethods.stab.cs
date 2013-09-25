using java.lang;

public static class C1 {
    public static string m1(this string s) {
        return s;
    }
}

public static class C2 {
    public static string m1(this string s) {
        return s;
    }
}

public class C3 {
    public string test() {
        var s = "str";
        return s.m1();
    }
}
