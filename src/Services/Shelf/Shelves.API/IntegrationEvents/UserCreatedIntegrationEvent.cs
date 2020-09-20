using Bookcase.BuildingBlocks.EventBus;
using System;

namespace Bookcase.Services.Shelves.API.IntegrationEvents
{
    public class UserCreatedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; set; }
    }
}
