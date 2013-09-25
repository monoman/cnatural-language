using java.lang;
using java.util;
using stab.query;

public class SelectMany {
	public static String test() {
		var obj1 = new SelectManyAux(1);
		var obj2 = new SelectManyAux(2);
		var lst = new ArrayList<SelectManyAux> { obj1, obj2 };
		var sb = new StringBuilder();
		foreach (var s in lst.selectMany(p => p.getStrings())) {
			sb.append(s);
		}
		return sb.toString();
	}
}

public class SelectManyAux {
	private int n;

	public SelectManyAux(int n) {
		this.n = n;
	}

	public Iterable<String> getStrings() {
		yield return "a" + n;
		yield return "b" + n;
		yield return "c" + n;
	}
}
