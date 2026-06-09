using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Convertly.Infrastructure.Persistence;

public sealed class ConvertlyDbContextFactory : IDesignTimeDbContextFactory<ConvertlyDbContext>
{
    public ConvertlyDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Database=convertly;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<ConvertlyDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ConvertlyDbContext(optionsBuilder.Options);
    }
}
