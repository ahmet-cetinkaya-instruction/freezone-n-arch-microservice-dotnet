using Application.Features.OrderItems.Constants;
using Application.Services.Repositories;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Domain.Entities;

namespace Application.Features.OrderItems.Rules;

public class OrderItemBusinessRules : BaseBusinessRules
{
    private readonly IOrderItemRepository _orderItemRepository;

    public OrderItemBusinessRules(IOrderItemRepository orderItemRepository)
    {
        _orderItemRepository = orderItemRepository;
    }

    public Task OrderItemShouldExistWhenSelected(OrderItem? orderItem)
    {
        if (orderItem == null)
            throw new BusinessException(OrderItemsBusinessMessages.OrderItemNotExists);
        return Task.CompletedTask;
    }

    public async Task OrderItemIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        OrderItem? orderItem = await _orderItemRepository.GetAsync(
            predicate: oi => oi.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await OrderItemShouldExistWhenSelected(orderItem);
    }
}