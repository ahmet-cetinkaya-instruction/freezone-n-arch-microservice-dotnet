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
            // Basket Service içerisinde
            // OrderCreated -> OrderCreatedEventHandlerForRemoveBasket
            // OrderCreated -> OrderCreatedEventHandlerForSendMail
            _handlers.Add(eventName, value: new List<SubscriptionInfo>());

        // Eğer bu event handler daha önce eklediyse
        if (_handlers[eventName].Any(subsInfo => subsInfo.HandlerType == integrationEventHandlerType))
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
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
    {
        string eventName = GetEventKey<TIntegrationEvent>();
        SubscriptionInfo? handlerToRemove = findSubscription(eventName, typeof(TIntegrationEventHandler));
        removeHandler(eventName, handlerToRemove);
    }

    private SubscriptionInfo? findSubscription(string eventName, Type handleType) =>
        _handlers[eventName].SingleOrDefault(subsInfo => subsInfo.HandlerType == handleType);

    private void removeHandler(string eventName, SubscriptionInfo? handlerToRemove)
    {
        // Handler tarafında remove işlemi
        if (handlerToRemove != null)
            _handlers[eventName].Remove(handlerToRemove);
        if (!_handlers[eventName].Any())
        {
            _handlers.Remove(eventName);

            // EventTypes tarafında remove işlemi
            Type? eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
            if (eventType != null)
                _eventTypes.Remove(eventType);
        }
        // OnEventRemoved event'i tetiklenir.
        raiseOnEventRemoved(eventName);
    }

    private void raiseOnEventRemoved(string eventName)
    {
        EventHandler<string> handler = OnEventRemoved; // OnEventRemoved event'i tetiklenir.
        handler?.Invoke(sender: this, eventName);
    }

    public void Clear()
    {
        _handlers.Clear();
    }

    public Type? GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(type => type.Name == eventName);

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<TIntegrationEvent>()
        where TIntegrationEvent : IntegrationEvent
    {
        string eventName = GetEventKey<TIntegrationEvent>();
        return GetHandlersForEvent(eventName);
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
        => _handlers[eventName]; // List<SubscriptionInfo>

    public string GetEventKey<TIntegrationEvent>()
        where TIntegrationEvent : IntegrationEvent
    {
        string eventName = typeof(TIntegrationEvent).Name; // Kod tarafında: class OrderCreatedIntegrationEvent
        return EventNameGetter(eventName); // RabbitMQ tarafında: OrderCreated
    }
}
