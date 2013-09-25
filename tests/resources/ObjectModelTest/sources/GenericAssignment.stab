using java.lang;

public class GenericAssignment<T> where T : Exception {
	public static bool test() {
		GenericAssignment<Exception> obj = new GenericAssignment<Exception>();
		obj.setT(new RuntimeException());
		return obj.e != null;
	}

	public Exception e*;
	
	public void setT(T t) {
		e = t;
	}
}
