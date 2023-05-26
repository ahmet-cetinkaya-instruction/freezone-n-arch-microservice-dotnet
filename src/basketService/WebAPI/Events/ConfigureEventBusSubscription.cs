using Application.Features.BasketItems.Events.OrderCreated;
using Core.EventBus.Abstraction;

namespace WebAPI.Events;

public static class ConfigureEventBusSubscription
{
    public static IServiceProvider ConfigureEventBusSubscriptions(this IServiceProvider serviceProvider, IHostApplicationLifetime lifetime)
    {
        IEventBus eventBus = serviceProvider.GetRequiredService<IEventBus>();

        subscribeEvents(eventBus);

        lifetime.ApplicationStopping.Register(() => unSubscribeEvents(eventBus));

        return serviceProvider;
    }

    private static void subscribeEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEvent.OrderCreatedIntegrationEventHandler>();
    }

    private static void unSubscribeEvents(IEventBus eventBus)
    {
        eventBus.Unsubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEvent.OrderCreatedIntegrationEventHandler>();
    }
}
