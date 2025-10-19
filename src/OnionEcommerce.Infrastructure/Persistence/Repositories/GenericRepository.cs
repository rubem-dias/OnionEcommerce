using Microsoft.EntityFrameworkCore;
using OnionEcommerce.Application.Interfaces.Repositories;

namespace OnionEcommerce.Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _context;

    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }
}