public class C {
	public void m(E e) {
		switch (e) {
		case A:
			break;
		case B:
			break;
		case A:
			break;
		}
	}
}

public enum E {
	A,
	B
}
