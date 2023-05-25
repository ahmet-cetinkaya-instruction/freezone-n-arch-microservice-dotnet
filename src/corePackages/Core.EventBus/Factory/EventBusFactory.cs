using Core.EventBus.Abstraction;
using Core.EventBus.Events;
using Core.EventBus.RabbitMQ;

namespace Core.EventBus.Factory;

public static class EventBusFactory
{
    public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider) =>
        config.EventBusType switch
        {
            // EventBusType.AzureServiceBus => new AzureServiceBusEventBus(config, serviceProvider),
            EventBusType.RabbitMQ
                => new RabbitMQEventBus(config, serviceProvider),
            _ => throw new ArgumentOutOfRangeException()
        };

    //switch(config.EventBusType)
    //{
    //    // case EventBusType.AzureServiceBus:
    //    //     return new AzureServiceBusEventBus(config, serviceProvider);
    //    case EventBusType.RabbitMQ:
    //        return new RabbitMQEventBus(config, serviceProvider);
    //    default:
    //        throw new ArgumentOutOfRangeException();
    //}
}
