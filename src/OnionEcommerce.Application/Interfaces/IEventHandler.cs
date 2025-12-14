namespace Onion.Ecommerce.Application.Interfaces;

/// <summary>
/// Interface genérica para handlers de eventos.
/// </summary>
/// <typeparam name="TEvent">Tipo do evento a ser tratado.</typeparam>
public interface IEventHandler<TEvent> where TEvent : class
{
    /// <summary>
    /// Processa o evento.
    /// </summary>
    Task HandleAsync(TEvent @event);
}
