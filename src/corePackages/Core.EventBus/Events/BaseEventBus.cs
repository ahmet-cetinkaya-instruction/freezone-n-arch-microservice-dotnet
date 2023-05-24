using Core.EventBus.Abstraction;
using Core.EventBus.SubsManager;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection;

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

        bool isProcessed = false;

        if (EventBusSubscriptionManager.HasSubscriptionsForEvent(eventName)) // İlgili event'e subscribe olunmuş mu, consume edecek miyiz?
        {
            // Bu event'e subscribe olan tüm handler'ları alıyoruz.
            IEnumerable<SubscriptionInfo> subscriptions = EventBusSubscriptionManager.GetHandlersForEvent(eventName);

            // Servislerin aynı scope kapsamında türetilmesini ve iletilmesini sağlamak için kullanılır.
            using (IServiceScope scope = ServiceProvider.CreateScope()) // Microsoft.Extensions.DependencyInjection.Abstraction
            {
                // Tüm handler'ları tek tek dolaşıyoruz.
                foreach (SubscriptionInfo subscriptionInfo in subscriptions)
                {
                    // Örn. OrderCreated için OrderCreatedIntegrationEventHandler
                    object? integrationEventHandler = scope.ServiceProvider.GetService(subscriptionInfo.HandlerType);
                    if (integrationEventHandler == null)
                        continue; // Servisler içerisinde kayıt yoksa diğer handler'dan devam edebiliriz.

                    //TODO: Event Type'ı alıcaz.
                    Type integrationEventType =
                        EventBusSubscriptionManager.GetEventTypeByName(
                            $"{EventBusConfig?.EventNamePrefix}{eventName}{EventBusConfig?.EventNameSuffix}" // Örn. OrderCreatedIntegrationEvent
                        ) ?? throw new InvalidOperationException("Event Type can not be null.");

                    //TODO: JSON olarak gelen message'ı Deserilize edicez. (IntegrationEvent)
                    object? integrationEvent = JsonConvert.DeserializeObject(value: message, type: integrationEventType);

                    //TODO: Handler sınıfını, interface'i tanımlıycaz. (reflection)
                    Type integrationEventHandlerConcreteType =
                        typeof(IIntegrationEventHandler<>).MakeGenericType(integrationEventType)
                        ?? throw new InvalidOperationException("Concrete Type can not be null.");

                    //TODO: İçerisindeki Handle metodunu tanımlaycaz,
                    MethodInfo integrationEventHandlerHandleMethod =
                        integrationEventHandlerConcreteType.GetMethod("Handle")
                        ?? throw new InvalidOperationException("Handle Method can not be null.");

                    //TODO: İlgili event parametresiyle beraber Handle metodunu çalıştırıcaz.
                    await (Task)
                        integrationEventHandlerHandleMethod.Invoke(obj: integrationEventHandler, parameters: new[] { integrationEvent })!;
                }
            }

            //TODO: Bu eventin herhangi bir handler tarafından işlenip işlenmediğini döndürücez.
            isProcessed = true;
        }

        return isProcessed;
    }

    public virtual void Dispose()
    {
        // EventBusConfig içerisindeki özellikle Connection objesini de dispose etmek için kullanılır.
        EventBusConfig = null!;
    }
}
