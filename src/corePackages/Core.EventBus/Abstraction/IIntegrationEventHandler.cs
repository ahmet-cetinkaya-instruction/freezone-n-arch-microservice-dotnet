using Core.EventBus.Events;

namespace Core.EventBus.Abstraction;

public interface IIntegrationEventHandler { } // Marker interface

public interface IIntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}
