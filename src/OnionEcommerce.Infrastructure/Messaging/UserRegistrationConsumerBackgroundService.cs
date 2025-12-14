using Onion.Ecommerce.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Onion.Ecommerce.Infrastructure.Messaging;

/// <summary>
/// Serviço que roda em background e consome mensagens da fila user-registration.
/// </summary>
public class UserRegistrationConsumerBackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public UserRegistrationConsumerBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void StartConsumer()
    {
        // Cria um scope para obter a dependência
        var scope = _serviceProvider.CreateScope();
        var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();

        // Inicia o consumer na fila user-registration em uma thread separada
        _ = Task.Run(() => consumer.StartConsuming("user-registration"));
    }
}
