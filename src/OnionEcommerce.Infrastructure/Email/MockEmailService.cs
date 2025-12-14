using Onion.Ecommerce.Application.Interfaces;

namespace Onion.Ecommerce.Infrastructure.Email;

public class MockEmailService : IEmailService
{
    public Task SendWelcomeEmailAsync(string email, string fullName)
    {
        // Simula envio de email (em produção seria via SMTP)
        Console.WriteLine($"?? Email de boas-vindas enviado!");
        Console.WriteLine($"   Para: {email}");
        Console.WriteLine($"   Nome: {fullName}");
        Console.WriteLine($"   Assunto: Bem-vindo à OnionEcommerce!");
        Console.WriteLine($"   Corpo: Obrigado por se registrar, {fullName}!");
        Console.WriteLine();

        return Task.CompletedTask;
    }
}
