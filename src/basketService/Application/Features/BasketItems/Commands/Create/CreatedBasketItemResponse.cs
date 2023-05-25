using Core.Application.Responses;

namespace Application.Features.BasketItems.Commands.Create;

public class CreatedBasketItemResponse : IResponse
{
    public string Id { get; set; }
    public int UserId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}