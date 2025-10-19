using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OnionEcommerce.Infrastructure.Persistence;

/// <summary>
/// This factory is used by the 'dotnet-ef' command-line tools to create a DbContext instance
/// at design time for tasks like creating and applying migrations. It is not used at runtime.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlite("Data Source=OnionEcommerce_Local.db");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}