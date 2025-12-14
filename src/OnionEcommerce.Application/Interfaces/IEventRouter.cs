namespace Onion.Ecommerce.Application.Interfaces;

/// <summary>
/// Interface para rotear eventos para seus respectivos handlers.
/// </summary>
public interface IEventRouter
{
    /// <summary>
    /// Roteia um evento para o handler apropriado baseado no tipo de evento.
    /// </summary>
    /// <param name="eventType">Tipo do evento (ex: "UserRegistered").</param>
    /// <param name="message">Conteúdo serializado do evento em JSON.</param>
    Task RouteAsync(string eventType, string message);
}
