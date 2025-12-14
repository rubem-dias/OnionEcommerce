using Onion.Ecommerce.Application.Events;
using Onion.Ecommerce.Application.Interfaces;
using System.Reflection;
using System.Text.Json;

namespace Onion.Ecommerce.Infrastructure.EventHandlers;

/// <summary>
/// Roteia eventos para seus handlers apropriados.
/// </summary>
public class EventRouter : IEventRouter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _eventTypeMap;

    public EventRouter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _eventTypeMap = MapEventTypes();
    }

    /// <summary>
    /// Mapeia nomes de eventos para seus tipos correspondentes.
    /// Procura por classes que herdam de um padrão específico.
    /// </summary>
    private Dictionary<string, Type> MapEventTypes()
    {
        var map = new Dictionary<string, Type>();

        // Encontra todos os tipos que terminam com "Event" no assembly
        var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => p.Name.EndsWith("Event") && p.Namespace?.Contains("Events") == true)
            .ToList();

        foreach (var eventType in eventTypes)
        {
            // Usa o nome da classe como chave (ex: UserRegisteredEvent -> UserRegistered)
            var key = eventType.Name.Replace("Event", string.Empty);
            map[key] = eventType;
        }

        return map;
    }

    public async Task RouteAsync(string eventType, string message)
    {
        // Tenta encontrar o tipo do evento pelo nome
        if (!_eventTypeMap.TryGetValue(eventType, out var mappedEventType))
        {
            Console.WriteLine($"?? Tipo de evento '{eventType}' não mapeado.");
            return;
        }

        try
        {
            // Deserializa o JSON para o tipo do evento
            var @event = JsonSerializer.Deserialize(message, mappedEventType);

            if (@event == null)
            {
                Console.WriteLine($"? Falha ao desserializar evento '{eventType}'");
                return;
            }

            // Encontra o handler para este tipo de evento
            var handlerInterfaceType = typeof(IEventHandler<>).MakeGenericType(mappedEventType);
            var handler = _serviceProvider.GetService(handlerInterfaceType);

            if (handler == null)
            {
                Console.WriteLine($"?? Nenhum handler registrado para o evento '{eventType}'");
                return;
            }

            // Invoca o método HandleAsync do handler
            var handleMethod = handlerInterfaceType.GetMethod("HandleAsync", 
                BindingFlags.Public | BindingFlags.Instance);

            if (handleMethod != null)
            {
                var task = (Task)handleMethod.Invoke(handler, new[] { @event });
                await task;

                Console.WriteLine($"? Evento '{eventType}' roteado e processado com sucesso");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Erro ao processar evento '{eventType}': {ex.Message}");
        }
    }
}
