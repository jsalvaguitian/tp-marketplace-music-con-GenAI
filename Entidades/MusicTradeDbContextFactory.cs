using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Entidades;

public class MusicTradeDbContextFactory : IDesignTimeDbContextFactory<MusicTradeDbContext>
{
    public MusicTradeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MusicTradeDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost\\SQLEXPRESS;Database=MusicTradeDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true" );

        return new MusicTradeDbContext(optionsBuilder.Options);
    }
}