using Microsoft.EntityFrameworkCore;
using Onion.Ecommerce.Domain.Entities;
using OnionEcommerce.Application.Interfaces.Repositories;
namespace OnionEcommerce.Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}