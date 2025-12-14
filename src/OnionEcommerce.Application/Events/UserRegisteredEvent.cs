namespace Onion.Ecommerce.Application.Events;

/// <summary>
/// Evento disparado quando um usuário se registra.
/// </summary>
public class UserRegisteredEvent
{
    public string EventType { get; set; } = "UserRegistered";
    public DateTime Timestamp { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
