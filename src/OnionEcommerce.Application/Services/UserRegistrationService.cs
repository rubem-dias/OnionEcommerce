using Onion.Ecommerce.Domain.Entities;
using OnionEcommerce.Application.Interfaces.Repositories;
using OnionEcommerce.Application.Interfaces.Security;
using OnionEcommerce.Application.Interfaces.Common;

namespace OnionEcommerce.Application.Services;

public class UserRegistrationService : IScopedService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IPasswordHasher _passwordHasher;

    public UserRegistrationService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;

        _passwordHasher = passwordHasher;
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

        return (true, "Usuário registrado com sucesso.");
    }
}