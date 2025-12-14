using Microsoft.EntityFrameworkCore;
using Onion.Ecommerce.Infrastructure.Messaging;
using OnionEcommerce.Application;          // Para o AddApplicationServices
using OnionEcommerce.Infrastructure;       // Para o AddInfrastructureServices
using OnionEcommerce.Infrastructure.Persistence;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// --- 1. REGISTRO DE SERVIÇOS ---

// Serviços Padrão da API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do RabbitMQ (seu código)
var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");
var factory = new ConnectionFactory
{
    HostName = rabbitMqConfig["HostName"],
    UserName = rabbitMqConfig["UserName"],
    Password = rabbitMqConfig["Password"],
    DispatchConsumersAsync = true
};
builder.Services.AddSingleton<IConnection>(sp => factory.CreateConnection());


// --- O AJUSTE VEM AQUI ---
// Configuração de Persistência Flexível
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
            // Lembre-se que já adicionamos o pacote Npgsql.EntityFrameworkCore.PostgreSQL
            options.UseNpgsql(connectionString);
            break;
        default:
            throw new NotSupportedException($"Provedor '{provider}' não é suportado.");
    }
});

// Registro dos Serviços das Camadas (Application & Infrastructure)
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// Registra o serviço que consome mensagens do RabbitMQ
builder.Services.AddScoped<UserRegistrationConsumerBackgroundService>();

// --- 2. CONSTRUÇÃO DA APLICAÇÃO ---
var app = builder.Build();

// --- 3. CONFIGURAÇÃO DO PIPELINE HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Inicia o consumer de RabbitMQ usando um escopo
using (var scope = app.Services.CreateScope())
{
    var consumerService = scope.ServiceProvider.GetRequiredService<UserRegistrationConsumerBackgroundService>();
    consumerService.StartConsumer();
}

// Hook para fechar a conexão do RabbitMQ de forma elegante
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    app.Services.GetService<IConnection>()?.Close();
});

// --- 4. EXECUÇÃO DA APLICAÇÃO ---
app.Run();