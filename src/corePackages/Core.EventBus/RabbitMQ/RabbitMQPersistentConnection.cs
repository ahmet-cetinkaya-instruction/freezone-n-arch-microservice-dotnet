using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace Core.EventBus.RabbitMQ;

public class RabbitMQPersistentConnection
{
    public bool IsConnected => _connection != null && _connection.IsOpen;

    // Her bir connection'ın açık olup olmadığını kontrol eder.
    private IConnection? _connection; // RabbitMQ.Client
    private readonly IConnectionFactory _connectionFactory; // RabbitMQ.Client
    private readonly int _retryCount;

    public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount)
    {
        _connectionFactory = connectionFactory;
        _retryCount = retryCount;
    }

    public bool TryConnect()
    {
        // Tekrar bağlanma süreçlerinde nasıl davranması gerektiğini belirtir.
        RetryPolicy connectionPolicy = Policy // Polly.Retry
            .Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetry(
                _retryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (ex, time) => { }
            );

        // Yeniden deneme politikasıyla bağlantı deneme gerçekleşir.
        connectionPolicy.Execute(() =>
        {
            _connection = _connectionFactory.CreateConnection();
        });

        return true;
    }
}
