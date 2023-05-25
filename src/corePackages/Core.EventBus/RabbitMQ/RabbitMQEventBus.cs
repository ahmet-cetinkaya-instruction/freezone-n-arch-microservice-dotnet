﻿using Core.EventBus.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

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

    public override void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
    {
        string eventName = typeof(TIntegrationEvent).Name;
        eventName = ProcessEventName(eventName);

        // Event için abonelik bulunursa
        if (!EventBusSubscriptionManager.HasSubscriptionsForEvent(eventName))
        {
            if (!_connection.IsConnected)
                _connection.TryConnect();

            string queueName = GetSubName(eventName);
            // RabbitMQ'da kuyruk tanımlamayı garantiye alalım. Oluşturulmadıysa oluşturucaz.
            _consumerChannel.QueueDeclare(
                queue: queueName, // Örn. OrderService.OrderCreated
                durable: true, // Kuyruğun kalıcı olmasını istiyoruz.
                exclusive: false, // Kuyruğun sadece bu bağlantı tarafından kullanılmamasını istiyoruz.
                autoDelete: false, // Kuyrukda mesaj kalmadığında kuyruğun silinmesini istemiyoruz.
                arguments: null
            ); // Kuyruk için ekstra argümanlar vermek istemiyoruz.

            // Queue ile Exchange'i biribirine bind ederek bağlıyoruz.
            _consumerChannel.QueueBind(
                queue: queueName, // Örn. OrderService.OrderCreated
                exchange: EventBusConfig?.DefaultTopicName,
                routingKey: eventName
            ); // Örn. OrderCreated
            // RabbitMQ tarafinda consume edeceğimizi belirtiyoruz.
        }

        // Abonelik yöneticisine abonelik ekleyerek proje tarafında consume edeceğimizi belirtiyoruz.
        EventBusSubscriptionManager.AddSubscription<TIntegrationEvent, TIntegrationEventHandler>();

        // Queue'ı consume etmeye başlıyoruz.
        startBasicConsume(eventName);
    }

    private void startBasicConsume(string eventName)
    {
        // Consumer Channel'ımız üzerinden consume edeceğiz.
        EventingBasicConsumer consumer = new(model: _consumerChannel);
        consumer.Received += Consumer_Received;

        _consumerChannel.BasicConsume(
            queue: GetSubName(eventName), // Örn. OrderService.OrderCreated
            autoAck: false, // Auto Acknowledge'ı devre dışı bırakıyoruz. Çünkü biz event'i process ettiğimizde acknowledge'ı biz göndereceğiz.
            consumer
        );
    }

    private void Consumer_Received(object? sender, BasicDeliverEventArgs eventArgs)
    {
        string eventName = eventArgs.RoutingKey;
        eventName = ProcessEventName(eventName);

        string message = Encoding.UTF8.GetString(eventArgs.Body.Span); // JSON string

        try
        {
            ProcessEvent(eventName, message).GetAwaiter();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Logging
        }
    }

    public override void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
    {
        EventBusSubscriptionManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
    }

    private void EventBusSubscriptionManager_OnEventRemoved(object? sender, string eventName)
    {
        // TODO: Implement
    }

    public override void Publish(IntegrationEvent @event)
    {
        if (!_connection.IsConnected)
            _connection.TryConnect();
    }
}
