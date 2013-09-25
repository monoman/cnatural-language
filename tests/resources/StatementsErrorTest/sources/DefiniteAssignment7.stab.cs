public class C {
	bool t;
	public void m() {
	 	bool b;

		if ((true ? t : (b = true)) && t) {
	    	if (b) ;        // compile-time error
		}
	}
}
