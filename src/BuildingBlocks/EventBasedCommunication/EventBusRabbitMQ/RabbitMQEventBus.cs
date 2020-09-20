using Bookcase.BuildingBlocks.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bookcase.BuildingBlocks.EventBusRabbitMQ
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IEventBusSubscriptionManager _subscriptionManager;
        private readonly IPersistentConnection _connection;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly IServiceProvider _serviceProvider;
        private IModel _consumerChannel;

        public RabbitMQEventBus(IEventBusSubscriptionManager subscriptionManager, IPersistentConnection connection,
            string exchangeName, string queueName, ILogger<RabbitMQEventBus> logger, IServiceProvider services)
        {
            _subscriptionManager = subscriptionManager;
            _subscriptionManager.OnEventRemoved += SubscriptionManager_OnEventRemoved;
            _connection = connection;
            _exchangeName = exchangeName;
            _queueName = queueName;
            _logger = logger;
            _consumerChannel = CreateConsumerChannel();
            _serviceProvider = services;
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
            _subscriptionManager.Clear();
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_connection.IsConnected)
            {
                if (!_connection.Connect())
                {
                    _logger.LogWarning($"Couldn't publish event {@event.Id}. No available connection was found.");
                    return;
                }
            }

            using (var ch = _connection.CreateModel())
            {
                DeclareExchange(ch);
                var msg = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType());
                ch.BasicPublish(_exchangeName,
                    routingKey: _subscriptionManager.GetEventKey(@event.GetType()),
                    body: msg);
            }
        }

        public void Subscribe<E, EH>()
            where E : IntegrationEvent
            where EH : IIntegrationEventHandler<E>
        {
            if (!_connection.IsConnected)
            {
                if (!_connection.Connect())
                {
                    var msg = $"No open connection. Event {typeof(E)} will be not published.";
                    _logger.LogWarning(msg);
                    throw new InvalidOperationException(msg);
                }
            }
            _subscriptionManager.AddSubscription<E, EH>();
            using (var ch = _connection.CreateModel())
            {
                ch.QueueBind(_queueName, _exchangeName, routingKey: _subscriptionManager.GetEventKey<E>());
            }
            StartConsume();
        }

        public void Unsubscribe<E, EH>()
            where E : IntegrationEvent
            where EH : IIntegrationEventHandler<E>
        {
            _subscriptionManager.RemoveSubscription<E, EH>();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
            {
                _connection.Connect();
                if (!_connection.IsConnected)
                {
                    _logger.LogCritical("Couldn't initialize RabbitMQ exchange and queue.");
                    return null;
                }
            }
            var ch = _connection.CreateModel();
            DeclareExchange(ch);
            ch.QueueDeclare(queue: _queueName);
            ch.CallbackException += (sender, eArgs) =>
            {
                _logger.LogWarning("RabbitMQ: Exception invoked by the model. Recreating consumer channel.");
                _consumerChannel?.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartConsume();
            };
            return ch;
        }

        private void StartConsume()
        {
            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                consumer.Received += ConsumerReceived;
                _consumerChannel.BasicConsume(_queueName, autoAck: false, consumer: consumer);
            }
        }

        private async Task ConsumerReceived(object sender, BasicDeliverEventArgs @event)
        {
            var msg = Encoding.UTF8.GetString(@event.Body.ToArray());
            await ProcessEvent(@event.RoutingKey, msg);
            _consumerChannel.BasicAck(@event.DeliveryTag, false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            var subscrInfo = _subscriptionManager.GetEventHandlersFor(eventName);
            using (var scope = _serviceProvider.CreateScope())
            {
                foreach (var info in subscrInfo)
                {
                    var handlerInstance = scope.ServiceProvider.GetService(info.HandlerType);
                    if (handlerInstance == null) continue;
                    object @event;
                    try
                    {
                        @event = JsonSerializer.Deserialize(message, info.EventType);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Deserialization of event {eventName} threw an exception.");
                        _logger.LogWarning(ex.ToString());
                        continue;
                    }
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(info.EventType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(handlerInstance, new object[] { @event });
                }
            }
        }

        private void DeclareExchange(IModel channel)
            => channel.ExchangeDeclare(_exchangeName, type: "direct");

        private void SubscriptionManager_OnEventRemoved(object sender, string e)
        {
            if (!_connection.IsConnected)
            {
                if (!_connection.Connect())
                {
                    _logger.LogWarning($"Couldn't unbind queue with binding key {e}. No available connection was found.");
                    return;
                }
            }

            using (var ch = _connection.CreateModel())
            {
                ch.QueueUnbind(queue: _queueName, exchange: _exchangeName, routingKey: e);
            }

            if (_subscriptionManager.IsEmpty)
            {
                _consumerChannel?.Close();
            }
        }
    }
}
