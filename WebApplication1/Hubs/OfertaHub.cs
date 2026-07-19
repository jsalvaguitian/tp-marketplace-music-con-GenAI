using System.Security.Claims;
using Enums;
using Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Hubs;

[Authorize]
public class OfertaHub : Hub
{
    private readonly MusicTradeDbContext _db;

    public OfertaHub(MusicTradeDbContext db)
    {
        _db = db;
    }

    public async Task JoinGroup(int publicacionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"publicacion-{publicacionId}");
    }

    public async Task EnviarOferta(int publicacionId, string mensaje)
    {
        if (string.IsNullOrWhiteSpace(mensaje))
            return;

        var usuarioId = int.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await _db.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
            return;

        var oferta = new Oferta
        {
            PublicacionId = publicacionId,
            UsuarioId = usuarioId,
            Mensaje = mensaje,
            FechaCreacion = DateTime.Now,
            Estado = EstadoOferta.Pendiente
        };

        _db.Ofertas.Add(oferta);
        await _db.SaveChangesAsync();

        await Clients.Group($"publicacion-{publicacionId}").SendAsync("RecibirOferta", new
        {
            oferta.Id,
            oferta.UsuarioId,
            UsuarioNombre = $"{usuario.Nombre} {usuario.Apellido}",
            oferta.Mensaje,
            oferta.FechaCreacion,
            oferta.PublicacionId
        });

        var publicacion = await _db.Publicaciones
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == publicacionId);
        if (publicacion != null && publicacion.UsuarioId != usuarioId)
        {
            await Clients.Group($"user-{publicacion.UsuarioId}").SendAsync("NuevaNotificacion", new
            {
                Tipo = "oferta",
                Mensaje = $"{usuario.Nombre} {usuario.Apellido} comentó la publicación \"{publicacion.Titulo}\"",
                Url = $"/Publicacion/Detalle/{publicacionId}"
            });
        }
    }

    public override async Task OnConnectedAsync()
    {
        if (Context.User?.Identity?.IsAuthenticated == true)
        {
            var usuarioId = int.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{usuarioId}");
        }
        await base.OnConnectedAsync();
    }
}
