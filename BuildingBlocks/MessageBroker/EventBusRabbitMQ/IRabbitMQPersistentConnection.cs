using System;
using RabbitMQ.Client;

namespace Team5.BuildingBlocks.MessageBroker.EventBusRabbitMQ
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();

    }
}