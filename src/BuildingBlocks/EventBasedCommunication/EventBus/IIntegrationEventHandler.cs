using System.Threading.Tasks;

namespace Bookcase.BuildingBlocks.EventBus
{
    public interface IIntegrationEventHandler<T> where T : IntegrationEvent
    {
        Task Handle(T @event);
    }
}
