using Core.Persistence.Repositories;

namespace Domain.Entities;

public class OrderItem : Entity<int>
{
    public int UserId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string Address { get; set; }
}
