using Microsoft.Extensions.DependencyInjection;
using Onion.Ecommerce.Application.Events;
using Onion.Ecommerce.Application.Interfaces;
using Onion.Ecommerce.Infrastructure.Email;
using Onion.Ecommerce.Infrastructure.EventHandlers;
using Onion.Ecommerce.Infrastructure.Messaging;
using OnionEcommerce.Application.Interfaces.Repositories;
using OnionEcommerce.Application.Interfaces.Security;
using OnionEcommerce.Infrastructure.Persistence;
using OnionEcommerce.Infrastructure.Persistence.Repositories;
using OnionEcommerce.Infrastructure.Security;

namespace OnionEcommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // 1. Registro do Unit of Work, Password Hasher, Message Publisher e Email Service
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
        services.AddScoped<IEmailService, MockEmailService>();

        // 2. Registro do Event Router e Handlers de Eventos
        services.AddScoped<IEventRouter, EventRouter>();
        // Registra handlers específicos para cada tipo de evento
        services.AddScoped<IEventHandler<UserRegisteredEvent>, UserRegisteredEventHandler>();

        // 3. Registro do Consumer de RabbitMQ
        services.AddScoped<IMessageConsumer, RabbitMqUserRegistrationConsumer>();

        // 4. Registro automático de todos os repositórios
        // (Este código procura por qualquer classe que termine com "Repository" e a registra)
        var repositoryAssembly = typeof(UserRepository).Assembly;
        var repositoryTypes = repositoryAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"))
            .ToList();

        foreach (var type in repositoryTypes)
        {
            var interfaceType = type.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{type.Name}");

            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, type);
            }
        }

        return services;
    }
}