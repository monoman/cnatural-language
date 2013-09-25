using java.lang;

public class InterfaceToString : InterfaceToStringAux {
	public override string toString() {
		return "OK";
	}
	
	public static string test() {
		InterfaceToStringAux i = new InterfaceToString();
		return i.toString();
	}
}

public interface InterfaceToStringAux {
}