using System.Security.Claims;
using Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicios;

namespace WebApplication1.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly IConversacionService _conversacionService;

    public ChatController(IConversacionService conversacionService)
    {
        _conversacionService = conversacionService;
    }

    public async Task<IActionResult> Index()
    {
        int usuarioId = ObtenerUsuarioIdLogueado();

        var conversaciones =
            await _conversacionService.ObtenerConversacionesUsuarioAsync(usuarioId);

        return View(conversaciones);
    }

    public async Task<IActionResult> Conversacion(int id)
    {
        var conversacion =
            await _conversacionService.ObtenerConversacionAsync(id);

        if (conversacion == null)
            return NotFound();

        return View(conversacion);
    }

    [HttpPost]
    public async Task<IActionResult> EnviarMensaje(int conversacionId, string texto)
    {
        if (!string.IsNullOrWhiteSpace(texto))
        {
            await _conversacionService.EnviarMensajeAsync(
                conversacionId,
                ObtenerUsuarioIdLogueado(),
                texto);
        }

        return RedirectToAction(nameof(Conversacion), new { id = conversacionId });
    }

    public async Task<IActionResult> CrearConversacion(int publicacionId)
    {
        var conversacion =
            await _conversacionService.CrearConversacionAsync(
                publicacionId,
                ObtenerUsuarioIdLogueado());

        return RedirectToAction(nameof(Conversacion),
            new { id = conversacion.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Cerrar(int id)
    {
        var result = await _conversacionService.CerrarConversacionAsync(
            id, ObtenerUsuarioIdLogueado());

        if (!result)
            return NotFound();

        TempData["MensajeExito"] = "Conversación cerrada.";
        return RedirectToAction(nameof(Conversacion), new { id });
    }

    public async Task<IActionResult> AceptarOferta(int publicacionId, int ofertanteId)
    {
        var conversacion =
            await _conversacionService.CrearConversacionAsync(
                publicacionId,
                ofertanteId);

        return RedirectToAction(nameof(Conversacion),
            new { id = conversacion.Id });
    }

    [HttpPost]
    public async Task<IActionResult> SubirArchivo(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return Json(new { ok = false, error = "Archivo vacío." });

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        var permitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".mp4", ".webm", ".mov" };
        if (!permitidas.Contains(extension))
            return Json(new { ok = false, error = "Tipo de archivo no permitido." });

        var maxSize = 50L * 1024 * 1024;
        if (archivo.Length > maxSize)
            return Json(new { ok = false, error = "El archivo supera los 50 MB." });

        var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "chat");
        Directory.CreateDirectory(dir);

        var nombre = $"{Guid.NewGuid()}{extension}";
        var ruta = Path.Combine(dir, nombre);

        await using (var stream = new FileStream(ruta, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        var url = $"/uploads/chat/{nombre}";
        return Json(new { ok = true, url });
    }

    private int ObtenerUsuarioIdLogueado()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}