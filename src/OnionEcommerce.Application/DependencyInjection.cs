using Microsoft.Extensions.DependencyInjection;
using OnionEcommerce.Application.Interfaces.Common;
using System.Reflection;

namespace OnionEcommerce.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        RegisterServices(services, assembly, typeof(IScopedService), ServiceLifetime.Scoped);

        RegisterServices(services, assembly, typeof(ITransientService), ServiceLifetime.Transient);

        RegisterServices(services, assembly, typeof(ISingletonService), ServiceLifetime.Singleton);

        return services;
    }

    private static void RegisterServices(IServiceCollection services, Assembly assembly, Type markerInterface, ServiceLifetime lifetime)
    {
        // Encontra todas as classes no assembly que implementam a marker interface
        var typesToRegister = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(markerInterface))
            .ToList();

        foreach (var type in typesToRegister)
        {
            // Tenta encontrar uma interface específica para a classe (ex: IMyService para MyService)
            // Ignorando as próprias marker interfaces
            var interfaceType = type
                .GetInterfaces()
                .FirstOrDefault(i => i != markerInterface && i.Name == $"I{type.Name}");

            // Se encontrou uma interface específica (IMyService), registra com ela.
            if (interfaceType != null)
            {
                services.Add(new ServiceDescriptor(interfaceType, type, lifetime));
            }
            else
            {
                services.Add(new ServiceDescriptor(type, type, lifetime));
            }
        }
    }
}