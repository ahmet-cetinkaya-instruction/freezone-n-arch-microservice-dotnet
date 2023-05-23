using Core.EventBus.Abstraction;
using Core.EventBus.SubsManager;

namespace Core.EventBus.Events;

public abstract class BaseEventBus : IEventBus
{
    public EventBusConfig EventBusConfig { get; set; }

    // İlgili Event Handler sınıflarını almak için kullanıcaz.
    public readonly IServiceProvider ServiceProvider;

    public readonly IEventBusSubscriptionManager EventBusSubscriptionManager;

    protected BaseEventBus(EventBusConfig eventBusConfig, IServiceProvider serviceProvider)
    {
        EventBusConfig = eventBusConfig ?? throw new ArgumentNullException(nameof(eventBusConfig), "EventBusConfig can not be null.");
        ServiceProvider = serviceProvider;
        EventBusSubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName); // Todo: Dependency Injection'dan alınması veya Konfigure edilmesi
    }

    public void Publish(IntegrationEvent @event) => throw new NotImplementedException();

    public void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent> => throw new NotImplementedException();

    public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent> => throw new NotImplementedException();

    protected virtual string ProcessEventName(string eventName) // Örn. OrderCreatedIntegrationEvent
    {
        if (EventBusConfig.DeleteEventPrefix) // false
            eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray());

        if (EventBusConfig.DeleteEventSuffix) // true
            eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray()); // OrderCreated

        return eventName;
    }
}
