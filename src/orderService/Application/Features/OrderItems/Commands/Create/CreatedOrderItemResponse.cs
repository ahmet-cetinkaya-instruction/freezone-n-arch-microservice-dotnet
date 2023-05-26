using Core.Application.Responses;

namespace Application.Features.OrderItems.Commands.Create;

public class CreatedOrderItemResponse : IResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public string Quantity { get; set; }
    public string Address { get; set; }
}