namespace Core.EventBus.Events;

public class EventBusConfig
{
    public EventBusType EventBusType { get; set; } = EventBusType.RabbitMQ;

    #region Connection
    // AMQP servisine kaç kere bağlanılmaya çalışılacağını belirtir.
    public int ConnectionRetryCount { get; set; } = 5;

    // Örn. RabbitMQ için connection string yeterli olacak.
    public string EventBusConnectionString { get; set; } = string.Empty;

    // ama diğer amqp servislerinde özel bir connection objesi kullanılabilir.
    public object? Connection { get; set; }
    #endregion

    #region Naming
    // Hangi servisin abone olacağını belirtir.
    public string SubscriberClientAppName { get; set; } = string.Empty;
    
    // Event isimlerinin önüne ve sonuna eklenecek prefix ve suffix'leri belirtir.
    public string EventNamePrefix { get; set; } = string.Empty;
    public string EventNameSuffix { get; set; } = "IntegrationEvent";
    #endregion

    #region Helper getters
    public bool DeleteEventPrefix => !string.IsNullOrEmpty(EventNamePrefix);
    public bool DeleteEventSuffix => !string.IsNullOrEmpty(EventNameSuffix);
    #endregion
}