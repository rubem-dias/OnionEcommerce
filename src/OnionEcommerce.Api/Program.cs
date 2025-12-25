using Microsoft.EntityFrameworkCore;
using Onion.Ecommerce.Infrastructure.Messaging;
using OnionEcommerce.Application;
using OnionEcommerce.Infrastructure;
using OnionEcommerce.Infrastructure.Persistence;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");
var factory = new ConnectionFactory
{
    HostName = rabbitMqConfig["HostName"],
    UserName = rabbitMqConfig["UserName"],
    Password = rabbitMqConfig["Password"],
    DispatchConsumersAsync = true
};
builder.Services.AddSingleton<IConnection>(sp => factory.CreateConnection());

var dbSettings = builder.Configuration.GetSection("DatabaseSettings");
var provider = dbSettings.GetValue<string>("Provider");
var connectionString = dbSettings.GetConnectionString(provider);

if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Configuração do banco de dados está incompleta.");
}

Console.WriteLine($"Usando o provedor de banco de dados: {provider}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    switch (provider)
    {
        case "Sqlite":
            options.UseSqlite(connectionString);
            break;
        case "Postgres":
            options.UseNpgsql(connectionString);
            break;
        default:
            throw new NotSupportedException($"Provedor '{provider}' não é suportado.");
    }
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddScoped<UserRegistrationConsumerBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var consumerService = scope.ServiceProvider.GetRequiredService<UserRegistrationConsumerBackgroundService>();
        consumerService.StartConsumer();
        Console.WriteLine("✅ RabbitMQ conectado e Consumer iniciado com sucesso.");
    }
    catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n⚠️  AVISO: Não foi possível conectar ao RabbitMQ.");
        Console.WriteLine("   -> Verifique se o container Docker está rodando.");
        Console.WriteLine("   -> A API continuará funcionando, mas o processamento de mensagens está INATIVO.\n");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n❌ Erro crítico ao iniciar RabbitMQ: {ex.Message}\n");
        Console.ResetColor();
    }
}

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    app.Services.GetService<IConnection>()?.Close();
});

app.Run();