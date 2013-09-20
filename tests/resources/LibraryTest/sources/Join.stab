using java.lang;
using java.util;
using stab.query;

public class Join {
	public static String test() {
		var owners = new ArrayList<JoinOwner> {
			new JoinOwner { Name = "A1" }, 
			new JoinOwner { Name = "A2" }, 
			new JoinOwner { Name = "A3" }
		};
		var owned = new ArrayList<JoinOwned> {
			new JoinOwned { Name = "B1", Owner = owners[0] },
			new JoinOwned { Name = "B2", Owner = owners[1] },
			new JoinOwned { Name = "B3", Owner = owners[1] },
			new JoinOwned { Name = "B4", Owner = owners[2] }
		};
		var s = "";
		foreach (var obj in owners.join(owned, owner => owner, owned => owned.Owner, (Owner, Owned) => new { Owner, Owned })) {
			s += "|" + obj.Owner.Name + ": " + obj.Owned.Name;
		}
		return s;
	}
}

public class JoinOwner {
	public String Name {
		get;
		set;
	}
}

public class JoinOwned {
	public String Name {
		get;
		set;
	}
	public JoinOwner Owner {
		get;
		set;
	}
}