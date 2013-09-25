using java.lang;
using stab.query;

public class C {
    public static void m(Iterable<Customers> customers) {
        var query = customers.join(orders, c => c.CustomerID, o => o.CustomerID, (c, o) => new { c, o }).join(details, query$id0 => query$id0.o.OrderID, d => d.OrderID, (query$id0, d) => new { query$id0, d }).join(products, query$id1 => query$id1.d.ProductID, p => p.ProductID, (query$id1, p) => new { query$id1.query$id0.c.Name, query$id1.query$id0.o.OrderDate, p.ProductName });
    }
}
