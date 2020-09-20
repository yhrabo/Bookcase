using System;

namespace Bookcase.BuildingBlocks.EventBus
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.UtcNow;
        }

        public IntegrationEvent(Guid id, DateTime dt)
        {
            Id = id;
            CreationTime = dt;
        }

        public Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
