using Onion.Ecommerce.Domain.Entities;

namespace OnionEcommerce.Application.Interfaces.Security
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
