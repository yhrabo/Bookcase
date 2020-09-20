using System;
using System.Collections.Generic;

namespace Bookcase.BuildingBlocks.EventBus
{
    public interface IEventBusSubscriptionManager
    {
        event EventHandler<string> OnEventRemoved;
        void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
        void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
        void Clear();
        IList<SubscriptionInfo> GetEventHandlersFor(string eventName);
        string GetEventKey<T>();
        string GetEventKey(Type t);
        bool IsEmpty { get; }
    }
}
