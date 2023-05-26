using Application.Services.Repositories;
using Core.EventBus.Abstraction;
using Core.EventBus.Events;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.BasketItems.Events.OrderCreated;

public class OrderCreatedIntegrationEvent : IntegrationEvent
{
    public int UserId { get; set; }
    public string Address { get; set; }
    public ICollection<BasketItem> BasketItems { get; set; } //TODO: Burada entity yerine dto gönderilmesi daha iyidir.

    public OrderCreatedIntegrationEvent() { }

    public OrderCreatedIntegrationEvent(int userId, string address, ICollection<BasketItem> basketItems)
    {
        UserId = userId;
        Address = address;
        BasketItems = basketItems;
    }

    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly IBasketItemRepository _basketItemRepository;
        private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

        public OrderCreatedIntegrationEventHandler(IBasketItemRepository basketItemRepository, ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            _basketItemRepository = basketItemRepository;
            _logger = logger;
        }

        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            List<BasketItem> basketItems = _basketItemRepository.Query().Where(bi => bi.UserId == @event.UserId).ToList();

            await _basketItemRepository.DeleteRangeAsync(basketItems);
            _logger.LogInformation($"Basket items of user with id {@event.UserId} deleted.");
        }
    }
}
