using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace Core.EventBus.RabbitMQ;

public class RabbitMQPersistentConnection : IDisposable
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

    // TryConnect birden fazla çağrılabilir. Bu noktada thread safe ilerleceğiz.
    private readonly object _lockObject = new();
    private bool _disposed;

    public bool TryConnect()
    {
        lock (_lockObject) // İkinci gelen iş parçacağı önceki beklemek durumunda olacak.
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

            // Bağlantı başarılı olmazsa false döner.
            if (!IsConnected)
                return false;

            // Bağlantı başarılı olursa çeşitli olaylarını dinleyeceğiz.
            _connection!.ConnectionShutdown += Connection_OnConnectionShutdown;
            _connection!.CallbackException += Connection_OnCallbackException;
            _connection!.ConnectionBlocked += Connection_OnConnectionBlocked;
            return true;
        }
    }

    public IModel CreateModel()
    {
        // Model oluşturulurken bağlantı kontrolü yapılır.
        if (!IsConnected)
            throw new InvalidOperationException("RabbitMQ bağlantısı yok.");
        // Model oluşturulur.
        return _connection!.CreateModel();
    }

    private void Connection_OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        // Bağlantı kapatıldığında tekrar bağlanmayı dener.

        // Logging da olabilir

        if (_disposed)
            return;

        TryConnect();
    }

    private void Connection_OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        // CallbackException: Bağlantıyı yeniden kurmaya çalışırken bir hata oluştuğunda tetiklenir.

        // Logging da olabilir

        if (_disposed)
            return;

        TryConnect();
    }

    private void Connection_OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        // Bağlantı engellendiğinde tekrar bağlanmayı dener.

        // Logging da olabilir

        if (_disposed)
            return;

        TryConnect();
    }

    public void Dispose()
    {
        _disposed = true;
        _connection?.Dispose();
    }
}
