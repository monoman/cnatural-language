using java.lang;

public static class C1 {
    public static String m1(this String s) {
        return s;
    }
}

public static class C2 {
    public static String m1(this String s) {
        return s;
    }
}

public class C3 {
    public String test() {
        var s = "str";
        return s.m1();
    }
}
