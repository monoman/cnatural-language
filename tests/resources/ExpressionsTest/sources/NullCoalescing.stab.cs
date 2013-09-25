using java.lang;

public class NullCoalescing {
	public static Object test(Object arg1, Object arg2) {
		return arg1 ?? arg2;
	}
}
