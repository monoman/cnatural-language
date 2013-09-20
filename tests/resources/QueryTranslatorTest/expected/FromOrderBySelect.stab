using java.lang;
using stab.query;

public class C {
    public static void m(Iterable<Customers> customers) {
        var query = customers.selectMany(c => c.Orders, (c, o) => new { c, o }).orderByDescending(query$id0 => query$id0.o.Total).select(query$id0 => new { query$id0.c.Name, query$id0.o.Total });
    }
}
