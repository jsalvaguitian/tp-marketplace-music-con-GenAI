using System.Security.Claims;
using Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly MusicTradeDbContext _db;

    public ChatHub(MusicTradeDbContext db)
    {
        _db = db;
    }

    public async Task JoinConversacion(int conversacionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversacion-{conversacionId}");
    }

    public async Task EnviarMensaje(int conversacionId, string? texto, string? archivoUrl = null)
    {
        texto = texto?.Trim();
        if (string.IsNullOrWhiteSpace(texto) && string.IsNullOrWhiteSpace(archivoUrl))
            return;

        var conversacion = await _db.Conversaciones
            .FirstOrDefaultAsync(c => c.Id == conversacionId);
        if (conversacion == null || conversacion.Cerrada)
            return;

        var idClaim = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idClaim == null) return;
        var usuarioId = int.Parse(idClaim);

        var usuario = await _db.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
            return;

        var mensaje = new Mensaje
        {
            ConversacionId = conversacionId,
            UsuarioId = usuarioId,
            Texto = texto ?? "",
            ArchivoUrl = archivoUrl,
            FechaEnvio = DateTime.Now
        };

        _db.Mensajes.Add(mensaje);
        await _db.SaveChangesAsync();

        await Clients.Group($"conversacion-{conversacionId}").SendAsync("RecibirMensaje", new
        {
            mensaje.Id,
            mensaje.UsuarioId,
            UsuarioNombre = $"{usuario.Nombre} {usuario.Apellido}",
            mensaje.Texto,
            mensaje.FechaEnvio,
            mensaje.ConversacionId,
            mensaje.ArchivoUrl
        });

        var otroId = conversacion.VendedorId == usuarioId
            ? conversacion.CompradorId
            : conversacion.VendedorId;

        await Clients.Group($"user-{otroId}").SendAsync("NuevaNotificacion", new
        {
            Tipo = "mensaje",
            Mensaje = $"{usuario.Nombre} te envió un mensaje",
            Url = $"/Chat/Conversacion/{conversacionId}"
        });
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
