using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Entidades;

public class MusicTradeDbContextFactory : IDesignTimeDbContextFactory<MusicTradeDbContext>
{
    public MusicTradeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MusicTradeDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=MusicTradeDB;User Id=sa;Password=Password123!;TrustServerCertificate=True;" );

        return new MusicTradeDbContext(optionsBuilder.Options);
    }
}