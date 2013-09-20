using java.lang;
using java.util;
using stab.query;

public class GroupJoinLINQ {
	public static String test() {
		var owners = new ArrayList<GroupJoinOwner> {
			new GroupJoinOwner { Name = "A1" }, 
			new GroupJoinOwner { Name = "A2" }, 
			new GroupJoinOwner { Name = "A3" }
		};
		var owned = new ArrayList<GroupJoinOwned> {
			new GroupJoinOwned { Name = "B1", Owner = owners[0] },
			new GroupJoinOwned { Name = "B2", Owner = owners[1] },
			new GroupJoinOwned { Name = "B3", Owner = owners[1] },
			new GroupJoinOwned { Name = "B4", Owner = owners[2] }
		};
		var query = from o1 in owners
					join o2 in owned on o1 equals o2.Owner into g
					select new { o1, g };
		var s = "";
		foreach (var obj in query) {
			s += "|" + obj.o1.Name + ": ";
			foreach (var o in obj.g) {
				s += o.Name + " ";
			}
		}
		return s;
	}
}

public class GroupJoinOwner {
	public String Name {
		get;
		set;
	}
}

public class GroupJoinOwned {
	public String Name {
		get;
		set;
	}
	public GroupJoinOwner Owner {
		get;
		set;
	}
}