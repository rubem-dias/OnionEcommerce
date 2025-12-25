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
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
        services.AddScoped<IEmailService, MockEmailService>();

        services.AddScoped<IEventRouter, EventRouter>();
        services.AddScoped<IEventHandler<UserRegisteredEvent>, UserRegisteredEventHandler>();

        services.AddScoped<IMessageConsumer, RabbitMqUserRegistrationConsumer>();

        var repositoryAssembly = typeof(InfrastructureAssembly).Assembly;
        
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