using java.lang;
using java.util;
using stab.query;

public class GroupBy2 {
	public static String test() {
		var list = new ArrayList<GroupBy2Aux> {
			new GroupBy2Aux { Key = "a", Value = "b" },
			new GroupBy2Aux { Key = "a", Value = "c" },
			new GroupBy2Aux { Key = "d", Value = "e" },
			new GroupBy2Aux { Key = "d", Value = "f" },
			new GroupBy2Aux { Key = "d", Value = "g" }
		};
		
		var result = "";
		foreach (var g in list.groupBy(p => p.Key, p => p.Value.toUpperCase())) {
			result += "(" + g.Key + ")";
			foreach (var e in g) {
				result += e;
			}
		}
		return result;
	}
}

public class GroupBy2Aux {
	public String Key {
		get;
		set;
	}
	
	public String Value {
		get;
		set;
	}
}