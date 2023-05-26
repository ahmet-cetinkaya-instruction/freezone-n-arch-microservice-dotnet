using Application.Services.Repositories;
using Domain.Entities;
using Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class OrderItemRepository : EfRepositoryBase<OrderItem, int, BaseDbContext>, IOrderItemRepository
{
    public OrderItemRepository(BaseDbContext context) : base(context)
    {
    }
}