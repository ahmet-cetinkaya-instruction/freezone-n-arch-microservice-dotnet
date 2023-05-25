using Core.Persistence.Repositories;

namespace Domain.Entities;

public class BasketItem : Entity<string>
{
    public int UserId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
