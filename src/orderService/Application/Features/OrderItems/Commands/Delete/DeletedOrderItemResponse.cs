using Core.Application.Responses;

namespace Application.Features.OrderItems.Commands.Delete;

public class DeletedOrderItemResponse : IResponse
{
    public int Id { get; set; }
}