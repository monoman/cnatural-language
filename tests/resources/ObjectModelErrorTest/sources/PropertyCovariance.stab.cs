public class B : A {
	public override A Prop {
		get {
			return null;
		}
	}
}

public class A {
	public virtual B Prop {
		get {
			return null;
		}
	}
}
