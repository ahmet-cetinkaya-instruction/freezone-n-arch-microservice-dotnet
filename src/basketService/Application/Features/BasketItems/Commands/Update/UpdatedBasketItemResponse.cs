using Core.Application.Responses;

namespace Application.Features.BasketItems.Commands.Update;

public class UpdatedBasketItemResponse : IResponse
{
    public string Id { get; set; }
    public int UserId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}