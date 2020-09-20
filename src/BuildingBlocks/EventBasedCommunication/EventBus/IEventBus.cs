namespace Bookcase.BuildingBlocks.EventBus
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);
        void Subscribe<E, EH>()
            where E : IntegrationEvent
            where EH : IIntegrationEventHandler<E>;
        void Unsubscribe<E, EH>()
            where E : IntegrationEvent
            where EH : IIntegrationEventHandler<E>;
    }
}
