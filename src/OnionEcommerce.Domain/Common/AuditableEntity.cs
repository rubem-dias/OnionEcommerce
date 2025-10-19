namespace Onion.Ecommerce.Domain.Common;

/// <summary>
/// Classe base para todas as entidades do sistema.
/// Contém propriedades comuns de auditoria.
/// </summary>
public abstract class AuditableEntity
{
    public Guid Id { get; protected set; }

    public DateTime CreatedAt { get; protected set; }

    public DateTime? UpdatedAt { get; protected set; }

    public bool IsDeleted { get; protected set; }

    protected AuditableEntity()
    {
        Id = Guid.NewGuid();
    }
}