using Core.EventBus.Abstraction;
using Core.EventBus.Events;

namespace Core.EventBus.SubsManager;

public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
{
    // Key: Event Name, Value: Handler List
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
    private readonly List<Type> _eventTypes;

    public Func<string, string> EventNameGetter { get; }

    public InMemoryEventBusSubscriptionManager(Func<string, string> eventNameGetter)
    {
        _handlers = new Dictionary<string, List<SubscriptionInfo>>();
        _eventTypes = new List<Type>();

        // OrderCreatedIntegrationEvent -> OrderCreated
        // Sınıf isimlerindeki fazlalıkları kaldırarak örn. RabbitMQ'a göndereceğiz. Çünkü RabbitMQ'da event isimleri kısa olmalı, ve ek olarak IntegrationEvent kelimesine gerek yok.
        EventNameGetter = eventNameGetter;
    }

    public bool IsEmpty => !_handlers.Keys.Any();
    public event EventHandler<string>? OnEventRemoved;

    public void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
    {
        string eventName = GetEventKey<TIntegrationEvent>();
        addSubscription(typeof(TIntegrationEventHandler), eventName);
        if (!_eventTypes.Contains(typeof(TIntegrationEvent)))
            _eventTypes.Add(typeof(TIntegrationEvent));
    }

    private void addSubscription(Type integrationEventHandlerType, string eventName)
    {
        // Eğer bu event daha önce eklenmediyse, event listesine eklenir.
        if (!HasSubscriptionsForEvent(eventName))
            _handlers.Add(eventName, value: new List<SubscriptionInfo>());

        // Eğer bu event handler daha önce eklediyse
        if (_handlers[eventName].Any(subsInfo => subsInfo.HandleType == integrationEventHandlerType))
            throw new ArgumentException(
                $"Bu event daha önce eklenmiş: {integrationEventHandlerType.Name}",
                paramName: nameof(integrationEventHandlerType)
            );

        _handlers[eventName].Add(new SubscriptionInfo(integrationEventHandlerType));
    }

    public bool HasSubscriptionsForEvent<TIntegrationEvent>()
        where TIntegrationEvent : IntegrationEvent
    {
        string eventName = GetEventKey<TIntegrationEvent>();
        return HasSubscriptionsForEvent(eventName);
    }

    public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

    public void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent> => throw new NotImplementedException();

    public void Clear() => throw new NotImplementedException();

    public Type? GetEventTypeByName(string eventName) => throw new NotImplementedException();

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<TIntegrationEvent>()
        where TIntegrationEvent : IntegrationEvent => throw new NotImplementedException();

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => throw new NotImplementedException();

    public string GetEventKey<TIntegrationEvent>()
        where TIntegrationEvent : IntegrationEvent
    {
        string eventName = typeof(TIntegrationEvent).Name; // OrderCreatedIntegrationEvent
        return EventNameGetter(eventName); // OrderCreated
    }
}
