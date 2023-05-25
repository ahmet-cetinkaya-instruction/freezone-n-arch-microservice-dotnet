using Core.EventBus.Events;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
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
            autoAck: false, // Auto Acknowledge'ı devre dışı bırakıyoruz. Çünkü biz event'i process ettiğimizde acknowledge'ı biz göndereceğiz. Satır no 114
            consumer
        );
    }

    private void Consumer_Received(object? sender, BasicDeliverEventArgs eventArgs)
    {
        // Event name'i alıyoruz.
        string eventName = eventArgs.RoutingKey;
        eventName = ProcessEventName(eventName);

        // Event mesaj byte dizisini string'e çeviriyoruz.
        string message = Encoding.UTF8.GetString(eventArgs.Body.Span); // JSON string

        // Event'i proje dahilinde ilgili handler sınıfını bulup process ediyoruz.
        try
        {
            ProcessEvent(eventName, message).GetAwaiter();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Logging
        }

        // Event'i process ettikten sonra acknowledge gönderiyoruz. Mesajın teslim alındığını ve işlediğimizi onaylıyoruz.
        _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    }

    public override void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
    {
        // SubsManager'dan aboneliği kaldırmak için bu metot yeterli.
        EventBusSubscriptionManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        // RabbitMQ tarafında da aboneliği kaldırmak için burada yapmak yerine, kaldırılığından emin olmak için
        // SubsManader'deki OnEventRemoved event'ini dinleyip, orada kaldırılmasını sağlayabiliriz.
    }

    private void EventBusSubscriptionManager_OnEventRemoved(object? sender, string eventName)
    {
        eventName = ProcessEventName(eventName);

        if (!_connection.IsConnected)
            _connection.TryConnect();

        _consumerChannel.QueueUnbind(queue: eventName, exchange: EventBusConfig?.DefaultTopicName, routingKey: eventName);

        if (!EventBusSubscriptionManager.IsEmpty)
            _consumerChannel.Close();
    }

    public override void Publish(IntegrationEvent @event)
    {
        if (!_connection.IsConnected)
            _connection.TryConnect();

        // Gönderme işlemlerinde sorun olmasına karşın retry mekanizması ekliyoruz.
        RetryPolicy policy = Policy
            .Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(
                retryCount: EventBusConfig.ConnectionRetryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (ex, time) => {
                    // Logging
                }
            );

        // Evet ismini alıyoruz.
        string eventName = @event.GetType().Name; // OrderCreatedIntegrationEvent
        eventName = ProcessEventName(eventName); // OrderCreated

        // Event'i JSON string'e çeviriyoruz.
        string message = JsonConvert.SerializeObject(@event);
        // Json string'i byte dizisine çeviriyoruz.
        byte[] body = Encoding.UTF8.GetBytes(message);

        // Event'i karşılayan bir exchange olmasını garantiye alıyoruz.
        _consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct"); //TODO: Exchange ismini ve type'ını dinamik olarak alabiliriz.

        // Event'i RabbitMQ'deki exchange'e publish ediyoruz.
        policy.Execute(() =>
        {
            IBasicProperties properties = _consumerChannel.CreateBasicProperties();
            properties.DeliveryMode = 2; // Persistent olarak işaretliyoruz. Yani RabbitMQ'da restart olsa bile mesajlarımız kaybolmayacak.

            // Kuyruğun tanımla (default)
            _consumerChannel.QueueDeclare(
                queue: GetSubName(eventName), // Örn. OrderService.OrderCreated
                durable: true, // Kuyruğun kalıcı olmasını istiyoruz.
                exclusive: false, // Kuyruğun sadece bu bağlantı tarafından kullanılmamasını istiyoruz.
                autoDelete: false, // Kuyrukda mesaj kalmadığında kuyruğun silinmesini istemiyoruz.
                arguments: null // Kuyruk için ekstra argümanlar vermek istemiyoruz.
            );

            // Event'i RabbitMQ tarafına publish edebiliriz.
            _consumerChannel.BasicPublish(
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName,
                mandatory: true, // Exchange'e publish edilemeyen mesajlar için basic.return event'i gönderilir.
                properties,
                body
            );
        });
    }
}
