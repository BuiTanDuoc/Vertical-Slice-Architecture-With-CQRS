using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CqrsDemo.Infrastructure.Persistence;

// Used only by `Add-Migration` / `Update-Database` (PMC) and `dotnet ef` at design time.
// Infrastructure has no appsettings.json of its own, so this reads the one from
// CqrsDemo.Api - which is fine since PMC's "Startup Project" is normally set to Api,
// meaning the working directory when this runs is already the Api project folder.
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = ResolveApiBasePath();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Default")
            ?? "Server=.;Database=CqrsDemo;Trusted_Connection=True;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sql =>
            sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

        return new AppDbContext(optionsBuilder.Options);
    }

    private static string ResolveApiBasePath()
    {
        var current = Directory.GetCurrentDirectory();

        // Covers the case where a command is run with Infrastructure as the working
        // directory (e.g. `dotnet ef` invoked directly from that folder) instead of
        // through the Api startup project.
        var apiCandidate = Path.Combine(current, "..", "CqrsDemo.Api");
        return Directory.Exists(apiCandidate) ? apiCandidate : current;
    }
}
