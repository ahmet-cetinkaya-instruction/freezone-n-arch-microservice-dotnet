using Core.EventBus.Events;

namespace Core.EventBus.Abstraction;

public interface IEventBus
{
    void Publish(IntegrationEvent @event);

    // Belirli bir türedeki IntegrationEvent'leri dinlemek için kullanılır. Olayları ilgili işleyiciye (handler) iletmek için kullanılır.
    void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

    // Belirli bir türedeki IntegrationEvent'leri dinlemekten vazgeçmek için kullanılır.
    void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
} 
