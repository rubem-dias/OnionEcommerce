using Onion.Ecommerce.Application.Events;
using Onion.Ecommerce.Application.Interfaces;

namespace Onion.Ecommerce.Infrastructure.EventHandlers;

/// <summary>
/// Handler que processa eventos de registro de usuário.
/// Envia um email de boas-vindas.
/// </summary>
public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
{
    private readonly IEmailService _emailService;

    public UserRegisteredEventHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task HandleAsync(UserRegisteredEvent @event)
    {
        // Envia email de boas-vindas
        await _emailService.SendWelcomeEmailAsync(@event.Email, @event.FullName);

        Console.WriteLine($"? Handler UserRegisteredEventHandler processou evento para: {@event.Email}");
    }
}
