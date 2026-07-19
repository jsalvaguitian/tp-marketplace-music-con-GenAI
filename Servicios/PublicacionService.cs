using Entidades;
using Enums;
using Microsoft.EntityFrameworkCore;

namespace Servicios
{
    public interface IPublicacionService
    {
        Task<List<Publicacion>> ObtenerActivasAsync();
        Task<Publicacion?> ObtenerPorIdAsync(int id);
        Task CrearAsync(Publicacion publicacion);
        Task<bool> EditarAsync(int id, int usuarioIdLogueado, Publicacion datosEditados, string? nuevaImagenUrl);

        Task CambiarEstadoAsync(int id, EstadoPublicacion nuevoEstado);
        Task<bool> CerrarAsync(int id, int usuarioIdLogueado);
    }

    public class PublicacionService : IPublicacionService
    {
        private readonly MusicTradeDbContext _context;

        public PublicacionService(MusicTradeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Publicacion>> ObtenerActivasAsync()
        {
            return await _context.Publicaciones
                .Include(p => p.Usuario)
                .Where(p => p.Estado == EstadoPublicacion.Activa || p.Estado == EstadoPublicacion.Finalizada)
                .ToListAsync();
        }

        public async Task<Publicacion?> ObtenerPorIdAsync(int id)
        {
            return await _context.Publicaciones
                .Include(p => p.Usuario)
                .Include(p => p.Ofertas)
                    .ThenInclude(o => o.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CrearAsync(Publicacion publicacion)
        {
            publicacion.FechaCreacion = DateTime.Now;
            publicacion.Estado = EstadoPublicacion.Activa;
            _context.Publicaciones.Add(publicacion);
            await _context.SaveChangesAsync();
        }

        public async Task CambiarEstadoAsync(int id, EstadoPublicacion nuevoEstado)
        {
            var publicacion = await _context.Publicaciones.FindAsync(id);
            if (publicacion == null) return;

            publicacion.Estado = nuevoEstado;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EditarAsync(int id, int usuarioIdLogueado, Publicacion datosEditados, string? nuevaImagenUrl)
        {
            var publicacion = await _context.Publicaciones.FindAsync(id);
            if (publicacion == null) return false;
            if (publicacion.UsuarioId != usuarioIdLogueado) return false;

            publicacion.Titulo = datosEditados.Titulo;
            publicacion.Descripcion = datosEditados.Descripcion;
            publicacion.FechaCierre = datosEditados.FechaCierre;
            publicacion.Categoria = datosEditados.Categoria;

            if (!string.IsNullOrEmpty(nuevaImagenUrl))
            {
                publicacion.ImagenUrl = nuevaImagenUrl;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CerrarAsync(int id, int usuarioIdLogueado)
        {
            var publicacion = await _context.Publicaciones.FindAsync(id);
            if (publicacion == null) return false;
            if (publicacion.UsuarioId != usuarioIdLogueado) return false;

            publicacion.Estado = EstadoPublicacion.Finalizada;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}