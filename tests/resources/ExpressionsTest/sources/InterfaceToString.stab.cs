using java.lang;

public class InterfaceToString : InterfaceToStringAux {
	public override String toString() {
		return "OK";
	}
	
	public static String test() {
		InterfaceToStringAux i = new InterfaceToString();
		return i.toString();
	}
}

public interface InterfaceToStringAux {
}