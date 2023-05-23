using Core.EventBus.Abstraction;
using Core.EventBus.SubsManager;
using Microsoft.Extensions.DependencyInjection;

namespace Core.EventBus.Events;

public abstract class BaseEventBus : IEventBus, IDisposable
{
    public EventBusConfig EventBusConfig { get; set; }

    // İlgili EventHandler sınıflarını almak için kullanıcaz.
    public readonly IServiceProvider ServiceProvider;

    // Abonelikleri yönetmek için kullanıcaz.
    public readonly IEventBusSubscriptionManager EventBusSubscriptionManager;

    protected BaseEventBus(EventBusConfig eventBusConfig, IServiceProvider serviceProvider)
    {
        EventBusConfig = eventBusConfig ?? throw new ArgumentNullException(nameof(eventBusConfig), "EventBusConfig can not be null.");
        ServiceProvider = serviceProvider;
        EventBusSubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName); // Todo: Dependency Injection'dan alınması veya Konfigure edilmesi
    }

    public abstract void Publish(IntegrationEvent @event);

    public abstract void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

    public abstract void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

    // Event isimlerinin önündeki ve sonundaki ekleri kaldırıyoruz.
    public virtual string ProcessEventName(string eventName) // Örn. OrderCreatedIntegrationEvent
    {
        if (EventBusConfig.DeleteEventPrefix) // false
            eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray());

        if (EventBusConfig.DeleteEventSuffix) // true
            eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray()); // OrderCreated

        return eventName;
    }

    // Bir subcription'ın tam ismini almak için kullanılır. Örn. OrderService.OrderCreated
    public virtual string GetSubName(string eventName) => $"{EventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";

    // Dışarıdan gelen AMQP mesajlarını işlemek için kullanılır.
    public async Task<bool> ProcessEvent(string eventName, string message)
    {
        eventName = ProcessEventName(eventName);

        if (EventBusSubscriptionManager
            .HasSubscriptionsForEvent(eventName)) // İlgili event'e subscribe olunmuş mu, consume edecek miyiz?
        {
            // Bu event'e subscribe olan tüm handler'ları alıyoruz.
            IEnumerable<SubscriptionInfo> subscriptions = EventBusSubscriptionManager
                .GetHandlersForEvent(eventName);

            // Servislerin aynı scope kapsamında türetilmesini ve iletilmesini sağlamak için kullanılır.
            using (IServiceScope scope = ServiceProvider.CreateScope()) // Microsoft.Extensions.DependencyInjection.Abstraction
            {
                // Tüm handler'ları tek tek dolaşıyoruz.
                foreach (SubscriptionInfo subscriptionInfo in subscriptions)
                {
                    // Örn. OrderCreated için OrderCreatedIntegrationEventHandler
                    object? handler = scope.ServiceProvider.GetService(subscriptionInfo.HandlerType);
                    if(handler == null) continue;  // Servisler içerisinde kayıt yoksa diğer handler'dan devam edebiliriz.
                    
                    //TODO: Event Type'ı alıcaz.
                    //TODO: JSON olarak gelen message'ı Deserilize edicez. (IntegrationEvent)
                    //TODO: Handler sınıfını, interface'i tanımlıycaz. (reflection)
                    //TODO: İçerisindeki Handle metodunu tanımlaycaz,
                    //TODO: İlgili event parametresiyle beraber Handle metodunu çalıştırıcaz.

                    //TODO: Bu eventin herhangi bir handler tarafından işlenip işlenmediğini döndürücez.
                }
            }
        }

        return true;
    }

    public virtual void Dispose()
    {
        // EventBusConfig içerisindeki özellikle Connection objesini de dispose etmek için kullanılır.
        EventBusConfig = null!;
    }
}
