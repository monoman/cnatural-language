using java.lang;

public class VarargsOverride {
	public static int test() {
		return new VarargsOverrideOther2().method1("s1", "s2");
	}
}

public class VarargsOverrideOther {
	
	    public virtual int method1 (String message, params Object[] args)
        {
                return 1;
        }
	
}

public class VarargsOverrideOther2 : VarargsOverrideOther {

        public override int method1 (String message, params Object[] args)
        {
                return 2;
        }
	
}

