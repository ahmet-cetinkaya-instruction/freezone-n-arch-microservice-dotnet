using Core.Application.Dtos;

namespace Application.Features.BasketItems.Queries.GetList;

public class GetListBasketItemListItemDto : IDto
{
    public string Id { get; set; }
    public int UserId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}