using java.lang;

public class AbstractProperty : AbstractPropertyAux {
	public static String test() {
		var obj = new AbstractProperty();
		return obj.Prop;
	}

	public override String Prop {
		get {
			return "STR";
		}
	}
}

public abstract class AbstractPropertyAux {
	public abstract String Prop {
		get;
	}
}
