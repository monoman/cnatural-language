using java.lang;
using stab.query;

public class C {
	public static void m(Iterable<Customers> customers) {
		var query = from c in customers
					join o in orders on c.CustomerID equals o.CustomerID
					join d in details on o.OrderID equals d.OrderID
					join p in products on d.ProductID equals p.ProductID
					select new { c.Name, o.OrderDate, p.ProductName };
	}
}
