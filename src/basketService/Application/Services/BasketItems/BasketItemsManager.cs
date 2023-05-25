using Application.Features.BasketItems.Rules;
using Application.Services.Repositories;
using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.BasketItems;

public class BasketItemsManager : IBasketItemsService
{
    private readonly IBasketItemRepository _basketItemRepository;
    private readonly BasketItemBusinessRules _basketItemBusinessRules;

    public BasketItemsManager(IBasketItemRepository basketItemRepository, BasketItemBusinessRules basketItemBusinessRules)
    {
        _basketItemRepository = basketItemRepository;
        _basketItemBusinessRules = basketItemBusinessRules;
    }

    public async Task<BasketItem?> GetAsync(
        Expression<Func<BasketItem, bool>> predicate,
        Func<IQueryable<BasketItem>, IIncludableQueryable<BasketItem, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        BasketItem? basketItem = await _basketItemRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return basketItem;
    }

    public async Task<IPaginate<BasketItem>?> GetListAsync(
        Expression<Func<BasketItem, bool>>? predicate = null,
        Func<IQueryable<BasketItem>, IOrderedQueryable<BasketItem>>? orderBy = null,
        Func<IQueryable<BasketItem>, IIncludableQueryable<BasketItem, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<BasketItem> basketItemList = await _basketItemRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return basketItemList;
    }

    public async Task<BasketItem> AddAsync(BasketItem basketItem)
    {
        BasketItem addedBasketItem = await _basketItemRepository.AddAsync(basketItem);

        return addedBasketItem;
    }

    public async Task<BasketItem> UpdateAsync(BasketItem basketItem)
    {
        BasketItem updatedBasketItem = await _basketItemRepository.UpdateAsync(basketItem);

        return updatedBasketItem;
    }

    public async Task<BasketItem> DeleteAsync(BasketItem basketItem, bool permanent = false)
    {
        BasketItem deletedBasketItem = await _basketItemRepository.DeleteAsync(basketItem);

        return deletedBasketItem;
    }
}
