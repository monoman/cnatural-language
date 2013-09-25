using java.lang;
using java.util;
using stab.query;

public class WhereSequenceEqual {
	public static string test() {
		var objects = new ArrayList<WhereSequenceEqual> {
			new WhereSequenceEqual("obj1", new ArrayList<string> { "a", "b" }),
			new WhereSequenceEqual("obj2", new ArrayList<string> { "c", "d" }),
			new WhereSequenceEqual("obj3", new ArrayList<string> { "e", "f" })
		};
		return objects.where(p => p.items.sequenceEqual(new ArrayList<string> { "c", "d" })).single().name;
	}
	
	public string name*;
	public List<string> items*;

	public WhereSequenceEqual(string name, List<string> items) {
		this.name = name;
		this.items = items;
	}
}
