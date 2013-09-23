public class IndexerInterface : IndexerInterfaceAux {
	private int[] t = new int[1];
	private java.lang.String last;
	
	public int this[int i] {
		get {
			return t[i];
		}
		set {
			t[i] = value;
		}
	}
	
	public int this[java.lang.String s] {
		get {
			last = s;
			return t[0];
		}
		set {
			last = s;
			t[0] = value;
		}
	}
	
	public int this[java.lang.String s, int i] {
		get {
			last = s;
			return t[i];
		}
		private set {
			last = s;
			t[i] = value;
		}
	}
	
	public int method() {
		IndexerInterfaceAux it = this;
		it[0] = 1;
		it["test1"] = 2;
		this["test2", 0] = 3;
		return it[0] + it["test3"] - it["test4", 0];
	}
	
	public static int test() {
		var obj = new IndexerInterface();
		return obj.method();
	}
}

public interface IndexerInterfaceAux {
	int this[int i] {
		get;
		set;
	}
	
	int this[java.lang.String s];

	int this[java.lang.String s, int i]^;

}
