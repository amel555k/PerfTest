namespace EStore.Domain.Entites;

public class Product
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Category { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}


/* Why domain ?

1. Domain is the heart i it doesn't know any of them

- Domain has any of the references (EF or ASP.NET or Npgsql)
- One day, if I want to switch from EF to Dapper, bacause that layer is Infrastructure (Separation of concerns)

2. Class (Entity) has identity and lives in base
3. DTO is the contract with outer world , exactly as API shows or recieves

4. class vs record deeper
- Class has reference identity: to Product objects with same params are always two idependent objects, EF can see the diff via key 
and follows them like like mutable objects
- Record has value equality and its native immutable  (for read-only messages)

5. required vs string? nullable reference types
- string without ? is "never null" by default, string? "can be null" [best way is to follow conventions form DB columns]
- required means that every time when creating that object, we must put explicitaly value to that field [in that case, no more = null or = ""]

6. program with smallest usable interface (ICollection<T> in this case)
- ICollection<T> is enough because the only thing we do with container is adding and iterating

7. Case Scenario is the calculations are been doing on DB layer so we dont implement methods for decreasing etc.

*/