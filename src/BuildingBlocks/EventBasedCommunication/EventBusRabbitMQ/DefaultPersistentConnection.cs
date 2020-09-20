using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;

namespace Bookcase.BuildingBlocks.EventBusRabbitMQ
{
    public class DefaultPersistentConnection : IPersistentConnection
    {
        private readonly object _connectingLock = new object();
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultPersistentConnection> _logger;
        private IConnection _connection;
        private bool _disposed;

        public DefaultPersistentConnection(IConnectionFactory connFactory, ILogger<DefaultPersistentConnection> logger)
        {
            _connectionFactory = connFactory;
            _logger = logger;
        }

        public bool IsConnected => (_connection != null) && _connection.IsOpen && !_disposed;

        public bool Connect()
        {
            if (!IsConnected)
            {
                lock (_connectingLock)
                {
                    if (!IsConnected)
                    {
                        try
                        {
                            _connection = _connectionFactory.CreateConnection();
                        }
                        catch (BrokerUnreachableException ex)
                        {
                            _logger.LogCritical("RabbitMQ Client couldn't create a connection.");
                            _logger.LogCritical(ex.Message);
                        }
                        if (IsConnected)
                        {
                            _logger.LogInformation("RabbitMQ Client established a connection.");
                            _connection.CallbackException += Connection_OnCallbackException;
                            _connection.ConnectionBlocked += Connection_OnConnectionBlocked;
                            return true;
                        }
                        else
                        {
                            _logger.LogError("RabbitMQ Client couldn't open a connection.");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("RabbitMQ Client isn't connected. The action cannot be performed.");
            }
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            try
            {
                _connection.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.ToString());
                return;
            }
            _logger.LogInformation("RabbitMQ Client disposed the connection.");
        }

        private void Connection_OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("RabbitMQ connection throw exception. Reconnecting...");
            Connect();
        }

        private void Connection_OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;
            _logger.LogWarning("RabbitMQ has blocked connection. Reconnecting...");
            Connect();
        }
    }
}
