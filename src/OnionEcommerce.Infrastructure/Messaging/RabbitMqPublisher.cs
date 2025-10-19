using Onion.Ecommerce.Application.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace Onion.Ecommerce.Infrastructure.Messaging;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConnection _connection;

    public RabbitMqPublisher(IConnection connection)
    {
        _connection = connection;
    }

    public void Publish(string queueName, string message)
    {
        using var channel = _connection.CreateModel();

        channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
    }
}