using Core.EventBus.Events;

namespace Core.EventBus.RabbitMQ;

public class RabbitMQEventBus : BaseEventBus
{
    private readonly RabbitMQPersistentConnection _connection;

    public RabbitMQEventBus(EventBusConfig eventBusConfig, IServiceProvider serviceProvider)
        : base(eventBusConfig, serviceProvider) { }

    public override void Publish(IntegrationEvent @event) => throw new NotImplementedException();

    public override void Subscribe<TIntegrationEvent, TIntegrationEventHandler>() => throw new NotImplementedException();

    public override void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>() => throw new NotImplementedException();
}