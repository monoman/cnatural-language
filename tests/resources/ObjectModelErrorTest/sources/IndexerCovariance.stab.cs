public class B : A {
	public override A this[int index] {
		get {
			return null;
		}
	}
}

public class A {
	public virtual B this[int index] {
		get {
			return null;
		}
	}
}
