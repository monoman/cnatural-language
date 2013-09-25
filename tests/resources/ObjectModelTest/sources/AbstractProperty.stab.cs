using java.lang;

public class AbstractProperty : AbstractPropertyAux {
	public static string test() {
		var obj = new AbstractProperty();
		return obj.Prop;
	}

	public override string Prop {
		get {
			return "STR";
		}
	}
}

public abstract class AbstractPropertyAux {
	public abstract string Prop {
		get;
	}
}
