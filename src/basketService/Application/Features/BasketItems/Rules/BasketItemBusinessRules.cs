using Application.Features.BasketItems.Constants;
using Application.Services.Repositories;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Domain.Entities;

namespace Application.Features.BasketItems.Rules;

public class BasketItemBusinessRules : BaseBusinessRules
{
    private readonly IBasketItemRepository _basketItemRepository;

    public BasketItemBusinessRules(IBasketItemRepository basketItemRepository)
    {
        _basketItemRepository = basketItemRepository;
    }

    public Task BasketItemShouldExistWhenSelected(BasketItem? basketItem)
    {
        if (basketItem == null)
            throw new BusinessException(BasketItemsBusinessMessages.BasketItemNotExists);
        return Task.CompletedTask;
    }

    public async Task BasketItemIdShouldExistWhenSelected(string id, CancellationToken cancellationToken)
    {
        BasketItem? basketItem = await _basketItemRepository.GetAsync(
            predicate: bi => bi.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await BasketItemShouldExistWhenSelected(basketItem);
    }
}