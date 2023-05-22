using Core.EventBus.Events;

namespace Core.EventBus.Abstraction;

public interface IEventBus
{
    void Publish(IntegrationEvent @event);


    // Belirli bir türedeki IntegrationEvent'leri dinlemek için kullanılır. Olayları ilgili işleyiciye (handler) iletmek için kullanılır.
    void Subscribe<TIntegrationEvent, IIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where IIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

    // Belirli bir türedeki IntegrationEvent'leri dinlemekten vazgeçmek için kullanılır.
    void Unsubscribe<TIntegrationEvent, IIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where IIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
} 
