using java.lang;
using stab.query;

public class C {
	public static void m(Iterable<Customers> customers) {
		var query = from c in customers
					from o in c.Orders
					orderby o.Total descending
					select new { c.Name, o.Total };

	}
}
