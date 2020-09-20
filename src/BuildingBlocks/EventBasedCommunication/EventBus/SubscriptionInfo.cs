using System;

namespace Bookcase.BuildingBlocks.EventBus
{
    public class SubscriptionInfo
    {
        public Type HandlerType { get; }
        public Type EventType { get; }

        public SubscriptionInfo(Type et, Type ht)
        {
            EventType = et;
            HandlerType = ht;
        }
    }
}
