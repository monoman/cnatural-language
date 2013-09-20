using java.lang;
using java.util;
using stab.query;

public class GroupJoin {
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
		var s = "";
		foreach (var obj in owners.groupJoin(owned, owner => owner, owned => owned.Owner, (Owner, OwnedList) => new { Owner, OwnedList })) {
			s += "|" + obj.Owner.Name + ": ";
			foreach (var o in obj.OwnedList) {
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