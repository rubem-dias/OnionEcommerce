using Onion.Ecommerce.Application.Events;
using Onion.Ecommerce.Application.Interfaces;
using Onion.Ecommerce.Domain.Entities;
using OnionEcommerce.Application.Interfaces.Repositories;
using OnionEcommerce.Application.Interfaces.Security;
using OnionEcommerce.Application.Interfaces.Common;
using System.Text.Json;

namespace OnionEcommerce.Application.Services;

public class UserRegistrationService : IScopedService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IPasswordHasher _passwordHasher;

    private readonly IMessagePublisher _messagePublisher;

    public UserRegistrationService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IMessagePublisher messagePublisher)
    {
        _unitOfWork = unitOfWork;

        _passwordHasher = passwordHasher;

        _messagePublisher = messagePublisher;
    }

    public async Task<(bool Success, string Message)> RegisterUserAsync(string fullName, string email, string password)
    {
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(email);

        if (existingUser != null)
            return (false, "Este e-mail já está em uso.");

        var passwordHash = _passwordHasher.Hash(password);

        var user = User.Create(fullName, email);
        user.SetPasswordHash(passwordHash);

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Publica evento de registro de usuário na fila user-registration
        var userRegisteredEvent = new UserRegisteredEvent
        {
            EventType = "UserRegistered",
            Timestamp = DateTime.UtcNow,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email
        };

        var jsonMessage = JsonSerializer.Serialize(userRegisteredEvent);
        _messagePublisher.Publish("user-registration", jsonMessage);

        return (true, "Usuário registrado com sucesso.");
    }
}