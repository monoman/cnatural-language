using java.lang;
using java.util;
using stab.query;

public class GroupByLINQ {
	public static String test() {
		var list = new ArrayList<GroupByAux> {
			new GroupByAux { Key = "a", Value = "b" },
			new GroupByAux { Key = "a", Value = "c" },
			new GroupByAux { Key = "d", Value = "e" },
			new GroupByAux { Key = "d", Value = "f" },
			new GroupByAux { Key = "d", Value = "g" }
		};
		var query = from g in list
					group g by g.Key;
		var result = "";
		foreach (var g in query) {
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