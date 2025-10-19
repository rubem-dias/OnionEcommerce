namespace Onion.Ecommerce.Application.Interfaces;

public interface IMessagePublisher
{
    /// <summary>
    /// Publica uma mensagem em uma fila específica.
    /// </summary>
    /// <param name="queueName">O nome da fila.</param>
    /// <param name="message">A mensagem a ser enviada (geralmente um JSON serializado).</param>
    void Publish(string queueName, string message);
}