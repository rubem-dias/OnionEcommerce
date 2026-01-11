using Onion.Ecommerce.Application.Interfaces;
using OnionEcommerce.Application.Interfaces.Common;
using OnionEcommerce.Application.Interfaces.Repositories;
using OnionEcommerce.Application.Interfaces.Security;
using System.Text.Json;

namespace OnionEcommerce.Application.Services;

public class UserLoginService : IScopedService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IPasswordHasher _passwordHasher;

    private readonly ITokenService _tokenService;

    private readonly IMessagePublisher _messagePublisher;

    public UserLoginService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IMessagePublisher messagePublisher
    )
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _messagePublisher = messagePublisher;
    }

    public async Task<(bool Success, string Message, string Token)> LoginAsync(string email, string password)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email);

        if (user == null)
            return (false, "E-mail ou senha inválidos.", null);

        bool isPasswordValid = _passwordHasher.Verify(password, user.PasswordHash);

        if (!isPasswordValid)
            return (false, "E-mail ou senha inválidos.", null);

        var token = _tokenService.GenerateToken(user);

        var userLoggedInEvent = new
        {
            EventType = "UserLoggedIn",
            Timestamp = DateTime.UtcNow,
            UserId = user.Id,
            Email = user.Email
        };

        var jsonMessage = JsonSerializer.Serialize(userLoggedInEvent);
        _messagePublisher.Publish("user-login", jsonMessage);

        return (true, "Login realizado com sucesso.", token);
    }
}