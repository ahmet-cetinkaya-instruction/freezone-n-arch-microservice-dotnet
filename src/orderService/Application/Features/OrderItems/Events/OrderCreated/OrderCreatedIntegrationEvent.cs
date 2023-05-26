using Application.Features.OrderItems.Dtos;
using Application.Services.Repositories;
using Core.EventBus.Abstraction;
using Core.EventBus.Events;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.OrderItems.Events.OrderCreated;

public class OrderCreatedIntegrationEvent : IntegrationEvent
{
    public int UserId { get; set; }
    public string Address { get; set; }
    public ICollection<BasketItemDto> BasketItems { get; set; }

    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

        public OrderCreatedIntegrationEventHandler(IOrderItemRepository orderItemRepository, ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            _orderItemRepository = orderItemRepository;
            _logger = logger;
        }

        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            ICollection<OrderItem> orderItemsToAdd = new List<OrderItem>();
            foreach (BasketItemDto basketItem in @event.BasketItems)
            {
                OrderItem orderItem = new OrderItem()
                {
                    UserId = @event.UserId,
                    Address = @event.Address,
                    ProductId = basketItem.ProductId,
                    ProductName = basketItem.ProductName,
                    UnitPrice = basketItem.UnitPrice,
                    Quantity = basketItem.Quantity,
                };
                orderItemsToAdd.Add(orderItem);
            }

            await _orderItemRepository.AddRangeAsync(orderItemsToAdd);
            _logger.LogInformation($"Order items ({orderItemsToAdd.Count}) added to database.");
        }
    }
}
