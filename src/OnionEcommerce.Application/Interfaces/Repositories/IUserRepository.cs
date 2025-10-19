using Onion.Ecommerce.Domain.Entities;
namespace OnionEcommerce.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}