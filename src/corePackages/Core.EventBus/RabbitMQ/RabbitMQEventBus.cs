using Core.EventBus.Events;
using RabbitMQ.Client;

namespace Core.EventBus.RabbitMQ;

public class RabbitMQEventBus : BaseEventBus
{
    private readonly RabbitMQPersistentConnection _connection;
    private readonly IConnectionFactory _connectionFactory;
    protected readonly IModel _consumerChannel;

    public RabbitMQEventBus(EventBusConfig eventBusConfig, IServiceProvider serviceProvider)
        : base(eventBusConfig, serviceProvider)
    {
        #region Creating Connection
        _connectionFactory = eventBusConfig.Connection != null ? (IConnectionFactory)eventBusConfig.Connection : new ConnectionFactory();
        _connection = new RabbitMQPersistentConnection(_connectionFactory, eventBusConfig.ConnectionRetryCount);
        #endregion

        #region Creating Channel
        _consumerChannel = createConsumerChannel();
        #endregion

        #region Add event to OnEventRemoved
        EventBusSubscriptionManager.OnEventRemoved += EventBusSubscriptionManager_OnEventRemoved;
        #endregion
    }

    private IModel createConsumerChannel()
    {
        if (!_connection.IsConnected)
            _connection.TryConnect();

        IModel channel = _connection.CreateModel();

        channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");

        return channel;
    }

    public override void Publish(IntegrationEvent @event)
    {
        if(!_connection.IsConnected)
            _connection.TryConnect();
        // TODO: Implement
    }

    public override void Subscribe<TIntegrationEvent, TIntegrationEventHandler>() => throw new NotImplementedException();

    public override void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
    {
        EventBusSubscriptionManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
    }

    private void EventBusSubscriptionManager_OnEventRemoved(object? sender, string eventName)
    {
        // TODO: Implement
    }
}
