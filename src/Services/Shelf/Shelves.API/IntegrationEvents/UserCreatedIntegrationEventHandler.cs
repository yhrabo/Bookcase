using Bookcase.BuildingBlocks.EventBus;
using Bookcase.Services.Shelves.API.Services;
using Bookcase.Services.Shelves.API.ViewModels;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookcase.Services.Shelves.API.IntegrationEvents
{
    public class UserCreatedIntegrationEventHandler
        : IIntegrationEventHandler<UserCreatedIntegrationEvent>
    {
        private readonly IShelfService _shelfService;
        private readonly ILogger<UserCreatedIntegrationEventHandler> _logger;

        public UserCreatedIntegrationEventHandler(IShelfService shelfService,
            ILogger<UserCreatedIntegrationEventHandler> logger)
        {
            _shelfService = shelfService;
            _logger = logger;
        }

        public async Task Handle(UserCreatedIntegrationEvent @event)
        {
            await _shelfService.AddShelfAsync(new UpsertShelfViewModel
                { AccessLevel = Models.AccessLevel.Private, Name = "Read" }, @event.UserId.ToString());
            await _shelfService.AddShelfAsync(new UpsertShelfViewModel
                { AccessLevel = Models.AccessLevel.Private, Name = "To read" }, @event.UserId.ToString());
            _logger.LogInformation($"Added shelves for user {@event.UserId.ToString()}.");
        }
    }
}
