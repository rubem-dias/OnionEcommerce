namespace Onion.Ecommerce.Application.Interfaces;

public interface IMessageConsumer
{
    /// <summary>
    /// Inicia a escuta de mensagens da fila especificada.
    /// </summary>
    /// <param name="queueName">O nome da fila a ser consumida.</param>
    void StartConsuming(string queueName);

    /// <summary>
    /// Para de escutar mensagens.
    /// </summary>
    void StopConsuming();
}
