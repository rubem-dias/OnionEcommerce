using Onion.Ecommerce.Domain.Common;

namespace Onion.Ecommerce.Domain.Entities;

public class User : AuditableEntity
{
    public string FullName { get; private set; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    private User(string fullName, string email)
    {
        FullName = fullName;
        Email = email;
    }

    public static User Create(string fullName, string email) => new(fullName, email);

    public void SetPasswordHash(string passwordHash) => PasswordHash = passwordHash;
}