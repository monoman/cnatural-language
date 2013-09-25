public class PropertyInterface : PropertyInterfaceAux {
	public int Property {
		get;
		set;
	}
	
	public int SugaredProperty;
	
	public int UltraSugaredProperty {
		get;
		private set;
	}
	
	public int method() {
		PropertyInterfaceAux it = this;
		it.Property = 1;
		it.SugaredProperty = 1;
		this.UltraSugaredProperty = 1;
		return it.Property + it.SugaredProperty + it.UltraSugaredProperty;
	}
	
	public static int test() {
		var obj = new PropertyInterface();
		return obj.method();
	}
}

public interface PropertyInterfaceAux {
	int Property {
		get;
		set;
	}
	
	int SugaredProperty;

	int UltraSugaredProperty^;
}

