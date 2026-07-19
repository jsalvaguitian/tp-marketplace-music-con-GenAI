using Entidades;
using Microsoft.EntityFrameworkCore;

namespace Servicios;

public interface IConversacionService
{
    Task<List<Conversacion>> ObtenerConversacionesUsuarioAsync(int usuarioId);

    Task<Conversacion?> ObtenerConversacionAsync(int conversacionId);

    Task EnviarMensajeAsync(int conversacionId, int usuarioId, string texto, string? archivoUrl = null);

    Task<Conversacion> CrearConversacionAsync(int publicacionId, int compradorId);

    Task<bool> CerrarConversacionAsync(int conversacionId, int usuarioId);
}

public class ConversacionService : IConversacionService
{
    private readonly MusicTradeDbContext _context;

    public ConversacionService(MusicTradeDbContext context)
    {
        _context = context;
    }

    public async Task<List<Conversacion>> ObtenerConversacionesUsuarioAsync(int usuarioId)
    {
        return await _context.Conversaciones
            .Include(c => c.Publicacion)
            .Include(c => c.Comprador)
            .Include(c => c.Vendedor)
            .Where(c => c.CompradorId == usuarioId || c.VendedorId == usuarioId)
            .ToListAsync();
    }

    public async Task<Conversacion?> ObtenerConversacionAsync(int conversacionId)
    {
        return await _context.Conversaciones
            .Include(c => c.Publicacion)
            .Include(c => c.Vendedor)
            .Include(c => c.Comprador)
            .Include(c => c.Mensajes)
                .ThenInclude(m => m.Usuario)
            .FirstOrDefaultAsync(c => c.Id == conversacionId);
    }

    public async Task EnviarMensajeAsync(int conversacionId, int usuarioId, string texto, string? archivoUrl = null)
    {
        var mensaje = new Mensaje
        {
            ConversacionId = conversacionId,
            UsuarioId = usuarioId,
            Texto = texto,
            ArchivoUrl = archivoUrl,
            FechaEnvio = DateTime.Now
        };

        _context.Mensajes.Add(mensaje);

        await _context.SaveChangesAsync();
    }

    public async Task<Conversacion> CrearConversacionAsync(int publicacionId, int compradorId)
    {
        var publicacion = await _context.Publicaciones.FindAsync(publicacionId);

        if (publicacion == null)
            throw new Exception("La publicación no existe.");

        var conversacionExistente = await _context.Conversaciones
            .FirstOrDefaultAsync(c =>
                c.PublicacionId == publicacionId &&
                c.CompradorId == compradorId);

        if (conversacionExistente != null)
            return conversacionExistente;

        var conversacion = new Conversacion
        {
            PublicacionId = publicacionId,
            CompradorId = compradorId,
            VendedorId = publicacion.UsuarioId
        };

        _context.Conversaciones.Add(conversacion);

        await _context.SaveChangesAsync();

        return conversacion;
    }

    public async Task<bool> CerrarConversacionAsync(int conversacionId, int usuarioId)
    {
        var conversacion = await _context.Conversaciones
            .FirstOrDefaultAsync(c => c.Id == conversacionId);

        if (conversacion == null || conversacion.VendedorId != usuarioId || conversacion.Cerrada)
            return false;

        conversacion.Cerrada = true;
        await _context.SaveChangesAsync();
        return true;
    }
}