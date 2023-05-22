using Core.EventBus.Events;

namespace Core.EventBus.Abstraction;

// Dışarıdan bize gönderilen subscription'ları yönetmek için kullanılır. InMemory veya Database gibi bir yerde tutulabilir.
public interface IEventBusSubscriptionManager
{
    bool IsEmpty { get; }

    // Bir olayın abonelikten kaldırılığını bildirmek için kullanılır.
    event EventHandler<string> OnEventRemoved;

    void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

    void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

    void Clear();

    bool HasSubscriptionsForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;
    bool HasSubscriptionsForEvent(string eventName);

    // Bir event'i ismiyle Type'ını almak için kullanılır.
    Type? GetEventTypeByName(string eventName);

    IEnumerable<SubscriptionInfo> GetHandlersForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;
    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

    // Bir event'in key'ini almak için kullanılır.
    string GetEventKey<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;
}
