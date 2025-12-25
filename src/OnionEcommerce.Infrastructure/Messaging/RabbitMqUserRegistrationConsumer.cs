using Onion.Ecommerce.Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Onion.Ecommerce.Infrastructure.Messaging;

public class RabbitMqUserRegistrationConsumer : IMessageConsumer
{
    private readonly IConnection _connection;
    private readonly IEventRouter _eventRouter;
    private IModel _channel;

    public RabbitMqUserRegistrationConsumer(IConnection connection, IEventRouter eventRouter)
    {
        _connection = connection;
        _eventRouter = eventRouter;
    }

    public void StartConsuming(string queueName)
    {
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.BasicQos(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var jsonMessage = Encoding.UTF8.GetString(body);

                Console.WriteLine($"\n?? Mensagem recebida da fila '{queueName}':");
                Console.WriteLine($"   Conteúdo: {jsonMessage}");

                using var doc = JsonDocument.Parse(jsonMessage);
                var root = doc.RootElement;

                var eventType = root.GetProperty("EventType").GetString();

                await _eventRouter.RouteAsync(eventType, jsonMessage);

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
    }

    public void StopConsuming()
    {
        _channel?.Close();
        _channel?.Dispose();
    }
}
