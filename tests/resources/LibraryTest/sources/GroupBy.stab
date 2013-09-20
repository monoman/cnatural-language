using java.lang;
using java.util;
using stab.query;

public class GroupBy {
	public static String test() {
		var list = new ArrayList<GroupByAux> {
			new GroupByAux { Key = "a", Value = "b" },
			new GroupByAux { Key = "a", Value = "c" },
			new GroupByAux { Key = "d", Value = "e" },
			new GroupByAux { Key = "d", Value = "f" },
			new GroupByAux { Key = "d", Value = "g" }
		};
		
		var result = "";
		foreach (var g in list.groupBy(p => p.Key)) {
			result += "(" + g.Key + ")";
			foreach (var e in g) {
				result += e.Value;
			}
		}
		return result;
	}
}

public class GroupByAux {
	public String Key {
		get;
		set;
	}
	
	public String Value {
		get;
		set;
	}
}