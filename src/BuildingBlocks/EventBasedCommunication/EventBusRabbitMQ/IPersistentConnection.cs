using RabbitMQ.Client;
using System;

namespace Bookcase.BuildingBlocks.EventBusRabbitMQ
{
    public interface IPersistentConnection : IDisposable
    {
        bool IsConnected { get; }
        bool Connect();
        IModel CreateModel();
    }
}
