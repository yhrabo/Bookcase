using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookcase.BuildingBlocks.EventBus
{
    public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
    {
        private readonly Dictionary<string, IList<SubscriptionInfo>> _handlers;

        public event EventHandler<string> OnEventRemoved;

        public InMemoryEventBusSubscriptionManager()
        {
            _handlers = new Dictionary<string, IList<SubscriptionInfo>>();
        }

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventKey = GetEventKey<T>();
            if (!_handlers.ContainsKey(eventKey))
            {
                _handlers.Add(eventKey, new List<SubscriptionInfo>());
            }

            var handlerType = typeof(TH);
            if (_handlers[eventKey].Any(info => info.HandlerType == handlerType))
            {
                return;
            }

            _handlers[eventKey].Add(new SubscriptionInfo(typeof(T), handlerType));
        }

        public void Clear()
        {
            _handlers.Clear();
        }

        public IList<SubscriptionInfo> GetEventHandlersFor(string eventName)
        {
            _handlers.TryGetValue(eventName, out var handlersInfo);
            return handlersInfo;
        }

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventKey = GetEventKey<T>();
            _handlers.TryGetValue(eventKey, out var eventHandlers);
            var handler = eventHandlers?.SingleOrDefault(info => info.HandlerType == typeof(TH));
            if (handler != null)
            {
                eventHandlers.Remove(handler);
                if (!eventHandlers.Any())
                {
                    OnEventRemoved?.Invoke(this, eventKey);
                }
            }
        }

        public string GetEventKey<T>() => typeof(T).Name;
        public string GetEventKey(Type t) => t.Name;
        public bool IsEmpty => !_handlers.Keys.Any();
    }
}
