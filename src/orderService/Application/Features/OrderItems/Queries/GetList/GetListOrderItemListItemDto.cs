using Core.Application.Dtos;

namespace Application.Features.OrderItems.Queries.GetList;

public class GetListOrderItemListItemDto : IDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public string Quantity { get; set; }
    public string Address { get; set; }
}