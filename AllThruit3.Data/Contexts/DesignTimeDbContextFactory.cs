using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AllThruit3.Data.Contexts;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AllThruitDbContext>
{
    public AllThruitDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AllThruitDbContext>();

        // Hardcode your LocalDB connection for dev/migrations (or parse args[0] for flexibility)
        var connectionString = "Server=localhost;Database=allThruit3;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
        if (args.Length > 0) connectionString = args[0]; // Optional: Pass custom string via command args

        optionsBuilder.UseSqlServer(connectionString);

        return new AllThruitDbContext(optionsBuilder.Options);
    }
}