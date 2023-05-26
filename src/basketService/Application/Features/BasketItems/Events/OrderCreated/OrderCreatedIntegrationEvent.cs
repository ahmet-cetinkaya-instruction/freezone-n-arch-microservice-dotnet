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
}
