using java.util;

public class WildcardBoundInheritance {
	public static bool test() {
		// Just test that it compiles
		method1(new ArrayList<WildcardBoundInheritanceAux2>());
		method2(new ArrayList<WildcardBoundInheritanceAux2>());
		return true;
	}
	
	static void method1(List<? : WildcardBoundInheritanceAux> list) {
	}
	
	static void method2(List<? : WildcardBoundInheritanceAux2> list) {
		method1(list);
	}
}

public class WildcardBoundInheritanceAux {

}

public class WildcardBoundInheritanceAux2 : WildcardBoundInheritanceAux {

}
