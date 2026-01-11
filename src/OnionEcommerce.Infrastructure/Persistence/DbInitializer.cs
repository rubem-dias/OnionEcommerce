using Microsoft.Extensions.DependencyInjection;
using Onion.Ecommerce.Domain.Entities;
using OnionEcommerce.Application.Interfaces.Security;
using OnionEcommerce.Domain.Enums;

namespace OnionEcommerce.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var hasher = serviceProvider.GetRequiredService<IPasswordHasher>();

        if (context.Users.Any())
            return;

        var admin = User.Create("Super Admin", "admin@onion.com", UserRole.Admin);

        var passwordHash = hasher.Hash("Admin@123");
        admin.SetPasswordHash(passwordHash);

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}