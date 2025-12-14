namespace Onion.Ecommerce.Application.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Envia um email de boas-vindas para um novo usuário.
    /// </summary>
    /// <param name="email">Email do usuário.</param>
    /// <param name="fullName">Nome completo do usuário.</param>
    Task SendWelcomeEmailAsync(string email, string fullName);
}
