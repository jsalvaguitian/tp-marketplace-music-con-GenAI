using Microsoft.EntityFrameworkCore;
using Entidades.Seed;

namespace Entidades;

public class MusicTradeDbContext : DbContext
{
    public MusicTradeDbContext(DbContextOptions<MusicTradeDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Provincia> Provincias { get; set; }
    public DbSet<Publicacion> Publicaciones { get; set; }
    public DbSet<Mensaje> Mensajes { get; set; }
    public DbSet<Oferta> Ofertas { get; set; }
    public DbSet<Conversacion> Conversaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Provincia)
            .WithMany(p => p.Usuarios)
            .HasForeignKey(u => u.ProvinciaId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Publicacion>()
            .HasOne(p => p.Usuario)
            .WithMany(u => u.Publicaciones)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Oferta>()
            .HasOne(o => o.Usuario)
            .WithMany()
            .HasForeignKey(o => o.UsuarioId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Oferta>()
            .HasOne(o => o.Publicacion)
            .WithMany(p => p.Ofertas)
            .HasForeignKey(o => o.PublicacionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Conversacion>()
            .HasOne(c => c.Publicacion)
            .WithMany(p => p.Conversaciones)
            .HasForeignKey(c => c.PublicacionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Conversacion>()
            .HasOne(c => c.Vendedor)
            .WithMany(u => u.ConversacionesComoVendedor)
            .HasForeignKey(c => c.VendedorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Conversacion>()
            .HasOne(c => c.Comprador)
            .WithMany(u => u.ConversacionesComoComprador)
            .HasForeignKey(c => c.CompradorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Mensaje>()
            .HasOne(m => m.Usuario)
            .WithMany(u => u.Mensajes)
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Mensaje>()
            .HasOne(m => m.Conversacion)
            .WithMany(c => c.Mensajes)
            .HasForeignKey(m => m.ConversacionId)
            .OnDelete(DeleteBehavior.NoAction);

        ProvinciasSeed.Seed(modelBuilder);
    }
}