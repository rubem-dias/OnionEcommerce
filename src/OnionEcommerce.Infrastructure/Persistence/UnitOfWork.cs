using OnionEcommerce.Application.Interfaces.Repositories;
using OnionEcommerce.Infrastructure.Persistence.Repositories;
namespace OnionEcommerce.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IUserRepository Users { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        Users = new UserRepository(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}