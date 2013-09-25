using java.lang;

public class EnumDeclaration {
	public static bool test() {
		return true;
	}
}

public enum EnumDeclarationAux {
	Field1(1),
	Field2(2);

	private EnumDeclarationAux(int value, params Object[] objs) {
	}
}
