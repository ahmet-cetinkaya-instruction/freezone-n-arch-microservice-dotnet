using Application.Services.Repositories;
using Domain.Entities;
using Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class BasketItemRepository : EfRepositoryBase<BasketItem, string, BaseDbContext>, IBasketItemRepository
{
    public BasketItemRepository(BaseDbContext context) : base(context)
    {
    }
}