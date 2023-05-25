using Core.Application.Responses;

namespace Application.Features.BasketItems.Commands.Delete;

public class DeletedBasketItemResponse : IResponse
{
    public string Id { get; set; }
}