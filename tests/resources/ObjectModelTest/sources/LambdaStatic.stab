using java.lang;

public class LambdaStatic {
	public static int test() {
		return method("ab");
	}
	
	static int method(String arg) {
		f1 = p => {
			int i = 0;
			i++;
			setF2(p.length() + i);
		};
		f1.call(arg);
		return f2;
	}
	
	static Func f1;
	static int f2;
	
	static void setF2(int val) {
		f2 = val;
	}
	
	interface Func {
		void call(String arg);
	}
}
