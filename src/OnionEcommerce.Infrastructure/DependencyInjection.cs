using Microsoft.Extensions.DependencyInjection;
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
        // 1. Registro do Unit of Work e Password Hasher
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // 2. Registro automático de todos os repositórios
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