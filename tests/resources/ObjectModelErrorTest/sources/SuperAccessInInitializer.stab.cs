public class A {
	public static int x = 1;
}

public class B : A {
	public B(): this(super.x) {
	}
	
	public B(int i) {
	}	
}
