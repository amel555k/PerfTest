namespace EStore.Domain.Entities;

public class Order
{
    public long Id { get; set; }
    public required string Customer { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}