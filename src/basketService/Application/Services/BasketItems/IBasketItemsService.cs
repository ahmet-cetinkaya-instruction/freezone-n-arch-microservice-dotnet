using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.BasketItems;

public interface IBasketItemsService
{
    Task<BasketItem?> GetAsync(
        Expression<Func<BasketItem, bool>> predicate,
        Func<IQueryable<BasketItem>, IIncludableQueryable<BasketItem, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<BasketItem>?> GetListAsync(
        Expression<Func<BasketItem, bool>>? predicate = null,
        Func<IQueryable<BasketItem>, IOrderedQueryable<BasketItem>>? orderBy = null,
        Func<IQueryable<BasketItem>, IIncludableQueryable<BasketItem, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<BasketItem> AddAsync(BasketItem basketItem);
    Task<BasketItem> UpdateAsync(BasketItem basketItem);
    Task<BasketItem> DeleteAsync(BasketItem basketItem, bool permanent = false);
}
