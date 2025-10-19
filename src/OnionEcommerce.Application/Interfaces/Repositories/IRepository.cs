namespace OnionEcommerce.Application.Interfaces.Repositories;

/// <summary>
/// Contrato genérico para repositórios.
/// </summary>
/// <typeparam name="TEntity">O tipo da entidade do domínio.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Busca uma entidade pelo seu Id.
    /// </summary>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todas as entidades.
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona uma nova entidade.
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca uma entidade como modificada para atualização.
    /// </summary>
    void Update(TEntity entity);

    /// <summary>
    /// Marca uma entidade para remoção.
    /// </summary>
    void Remove(TEntity entity);
}