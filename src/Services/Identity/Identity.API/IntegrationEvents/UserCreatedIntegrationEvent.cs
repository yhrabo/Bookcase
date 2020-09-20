using Bookcase.BuildingBlocks.EventBus;
using System;

namespace Identity.API.IntegrationEvents
{
    public class UserCreatedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; private set; }

        public UserCreatedIntegrationEvent(string userId)
        {
            UserId = userId;
        }
    }
}
