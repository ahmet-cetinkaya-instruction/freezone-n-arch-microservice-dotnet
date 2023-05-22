namespace Core.EventBus;

public class SubscriptionInfo
{
    // Integration event handler'in tipini burada tutucağız. Type üzerinden Handle metoduna ulaşabileceğiz.
    public Type HandleType { get; }

    public SubscriptionInfo(Type handleType)
    {
        HandleType = handleType;
    }
}
