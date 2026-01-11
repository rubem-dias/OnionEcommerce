using Onion.Ecommerce.Domain.Common;
using OnionEcommerce.Domain.Enums;

namespace Onion.Ecommerce.Domain.Entities;

public class User : AuditableEntity
{
    public string FullName { get; private set; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public UserRole Role { get; private set; }

    private User(string fullName, string email, UserRole role)
    {
        FullName = fullName;
        Email = email;
        Role = role;
    }

    public static User Create(string fullName, string email, UserRole role = UserRole.User) => new(fullName, email, role);

    public void SetPasswordHash(string passwordHash) => PasswordHash = passwordHash;
}