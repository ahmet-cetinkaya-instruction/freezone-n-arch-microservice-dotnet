namespace Core.EventBus;

public class SubscriptionInfo
{
    // Integration event handler'in tipini burada tutucağız. Type üzerinden Handle metoduna ulaşabileceğiz.
    public Type HandlerType { get; }

    public SubscriptionInfo(Type handlerType)
    {
        HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType), "Handler type must not be null.");
    }
}
